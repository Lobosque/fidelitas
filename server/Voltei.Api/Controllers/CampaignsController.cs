using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voltei.Api.Data;
using Voltei.Api.Dto;
using Voltei.Api.Models;
using Voltei.Api.Services.Wallet;

namespace Voltei.Api.Controllers;

[ApiController]
[Route("api/campaigns")]
[Authorize]
public class CampaignsController(AppDbContext db, GoogleWalletService googleWallet) : ControllerBase
{
    private Guid GetNegocioId() =>
        Guid.Parse(User.FindFirstValue("negocioId")!);

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var negocioId = GetNegocioId();

        var campaigns = await db.Campaigns
            .Where(c => c.NegocioId == negocioId)
            .OrderByDescending(c => c.CriadaEm)
            .ToListAsync();

        return Ok(campaigns.Select(ToCampaignResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCampaignRequest request)
    {
        var negocioId = GetNegocioId();

        var business = await db.Businesses.FindAsync(negocioId);

        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            NegocioId = negocioId,
            Nome = request.Nome,
            Descricao = request.Descricao,
            CheckinsNecessarios = request.CheckinsNecessarios,
        };

        db.Campaigns.Add(campaign);
        await db.SaveChangesAsync();

        // Criar Loyalty Class no Google Wallet (se configurado)
        if (business != null)
        {
            var classId = await googleWallet.CreateLoyaltyClassAsync(campaign, business);
            if (classId != null)
            {
                campaign.WalletClassId = classId;
                await db.SaveChangesAsync();
            }
        }

        return Created($"/api/campaigns/{campaign.Id}", ToCampaignResponse(campaign));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var negocioId = GetNegocioId();

        var campaign = await db.Campaigns
            .FirstOrDefaultAsync(c => c.Id == id && c.NegocioId == negocioId);

        if (campaign == null)
            return NotFound(new { message = "Campanha não encontrada." });

        return Ok(ToCampaignResponse(campaign));
    }

    private static CampaignResponse ToCampaignResponse(Campaign c) => new()
    {
        Id = c.Id.ToString(),
        NegocioId = c.NegocioId.ToString(),
        Nome = c.Nome,
        Descricao = c.Descricao,
        CheckinsNecessarios = c.CheckinsNecessarios,
        Ativa = c.Ativa,
        CriadaEm = c.CriadaEm.ToString("o"),
        WalletClassId = c.WalletClassId,
    };
}
