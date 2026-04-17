using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voltei.Api.Dto;
using Voltei.Api.Services;

namespace Voltei.Api.Controllers;

[ApiController]
[Route("api/checkins")]
[Authorize]
public class CheckinController(CheckinService checkinService) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private Guid GetNegocioId() =>
        Guid.Parse(User.FindFirstValue("negocioId")!);

    [HttpPost]
    public async Task<IActionResult> RegisterCheckin(CheckinRequest request)
    {
        var staffId = GetUserId();
        var (response, error) = await checkinService.RegisterCheckinAsync(request.EnrollmentToken, staffId);

        if (error != null)
            return BadRequest(new { message = error });

        return Ok(response);
    }

    [HttpPost("{enrollmentId:guid}/redeem")]
    public async Task<IActionResult> Redeem(Guid enrollmentId)
    {
        var negocioId = GetNegocioId();
        var error = await checkinService.RedeemAsync(enrollmentId, negocioId);

        if (error != null)
            return BadRequest(new { message = error });

        return Ok(new { message = "Prêmio resgatado com sucesso!" });
    }
}
