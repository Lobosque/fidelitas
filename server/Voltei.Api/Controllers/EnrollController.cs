using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voltei.Api.Data;
using Voltei.Api.Dto;
using Voltei.Api.Services;
using Voltei.Api.Services.Wallet;

namespace Voltei.Api.Controllers;

[ApiController]
[Route("api/enroll")]
public class EnrollController(
    EnrollmentService enrollmentService,
    AppleWalletService appleWallet,
    AppDbContext db) : ControllerBase
{
    [HttpGet("{campaignId:guid}")]
    public async Task<IActionResult> GetCampaignInfo(Guid campaignId)
    {
        var result = await enrollmentService.GetCampaignWithBusinessAsync(campaignId);
        if (result == null)
            return NotFound(new { message = "Campanha não encontrada." });

        var (campaign, business) = result.Value;

        return Ok(new CampaignPublicResponse
        {
            Id = campaign.Id.ToString(),
            Nome = campaign.Nome,
            Descricao = campaign.Descricao,
            CheckinsNecessarios = campaign.CheckinsNecessarios,
            NegocioNome = business.Nome,
            LogoUrl = business.LogoUrl,
            CoresPrimaria = business.CoresPrimaria,
            CoresSecundaria = business.CoresSecundaria,
        });
    }

    [HttpPost("{campaignId:guid}")]
    public async Task<IActionResult> Enroll(Guid campaignId, EnrollRequest request)
    {
        var campaignResult = await enrollmentService.GetCampaignWithBusinessAsync(campaignId);
        if (campaignResult == null)
            return NotFound(new { message = "Campanha não encontrada." });

        var (campaign, _) = campaignResult.Value;
        var (enrollment, alreadyEnrolled, googleSaveUrl) =
            await enrollmentService.EnrollAsync(campaignId, request);

        string? applePassUrl = null;
        if (!string.IsNullOrEmpty(enrollment.ApplePassSerial))
        {
            applePassUrl = $"/api/enroll/{enrollment.Id}/pass.pkpass";
        }

        return Ok(new EnrollResponse
        {
            EnrollmentId = enrollment.Id.ToString(),
            CheckinsAtuais = enrollment.CheckinsAtuais,
            CheckinsNecessarios = campaign.CheckinsNecessarios,
            AlreadyEnrolled = alreadyEnrolled,
            GoogleWalletSaveUrl = googleSaveUrl,
            ApplePassUrl = applePassUrl,
        });
    }

    [HttpGet("{enrollmentId:guid}/pass.pkpass")]
    public async Task<IActionResult> DownloadPass(Guid enrollmentId)
    {
        var enrollment = await db.Enrollments
            .Include(e => e.Cliente)
            .Include(e => e.Campanha)
                .ThenInclude(c => c.Negocio)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);

        if (enrollment == null)
            return NotFound(new { message = "Inscrição não encontrada." });

        var passBytes = appleWallet.GeneratePass(
            enrollment, enrollment.Campanha, enrollment.Campanha.Negocio, enrollment.Cliente);

        if (passBytes == null)
            return StatusCode(503, new { message = "Apple Wallet não disponível." });

        return File(passBytes, "application/vnd.apple.pkpass", "voltei.pkpass");
    }

    [HttpGet("{campaignId:guid}/status")]
    public async Task<IActionResult> CheckStatus(Guid campaignId, [FromQuery] string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return BadRequest(new { message = "Telefone é obrigatório." });

        var campaignResult = await enrollmentService.GetCampaignWithBusinessAsync(campaignId);
        if (campaignResult == null)
            return NotFound(new { message = "Campanha não encontrada." });

        var enrollment = await enrollmentService.FindExistingEnrollmentAsync(campaignId, telefone);
        if (enrollment == null)
            return NotFound(new { message = "Inscrição não encontrada." });

        return Ok(new EnrollResponse
        {
            EnrollmentId = enrollment.Id.ToString(),
            CheckinsAtuais = enrollment.CheckinsAtuais,
            CheckinsNecessarios = campaignResult.Value.campaign.CheckinsNecessarios,
            AlreadyEnrolled = true,
            GoogleWalletSaveUrl = null,
            ApplePassUrl = !string.IsNullOrEmpty(enrollment.ApplePassSerial)
                ? $"/api/enroll/{enrollment.Id}/pass.pkpass"
                : null,
        });
    }
}
