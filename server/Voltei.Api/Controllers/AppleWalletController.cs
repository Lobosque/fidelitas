using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voltei.Api.Data;
using Voltei.Api.Models;
using Voltei.Api.Services.Wallet;

namespace Voltei.Api.Controllers;

/// <summary>
/// Endpoints exigidos pela Apple PassKit Web Service Reference.
/// O iOS chama esses endpoints para registrar devices, buscar passes atualizados e enviar logs.
/// https://developer.apple.com/documentation/walletpasses/adding-a-web-service-to-update-passes
/// </summary>
[ApiController]
[Route("api/apple-wallet/v1")]
public class AppleWalletController(AppDbContext db, AppleWalletService appleWallet) : ControllerBase
{
    /// <summary>
    /// Registrar device para receber push updates de um pass.
    /// POST /v1/devices/{deviceId}/registrations/{passTypeId}/{serialNumber}
    /// </summary>
    [HttpPost("devices/{deviceId}/registrations/{passTypeId}/{serialNumber}")]
    public async Task<IActionResult> RegisterDevice(
        string deviceId, string passTypeId, string serialNumber)
    {
        var authToken = ExtractAuthToken();
        if (authToken == null) return Unauthorized();

        var enrollment = await db.Enrollments
            .FirstOrDefaultAsync(e =>
                e.ApplePassSerial == serialNumber &&
                e.ApplePassAuthToken == authToken);

        if (enrollment == null) return Unauthorized();

        // Extrair push token do body
        string? pushToken = null;
        try
        {
            var body = await Request.ReadFromJsonAsync<DeviceRegistrationBody>();
            pushToken = body?.PushToken;
        }
        catch { /* body pode ser vazio */ }

        // Salvar push token no enrollment
        if (!string.IsNullOrEmpty(pushToken))
        {
            enrollment.ApplePushToken = pushToken;
        }

        // Checar se registration já existe
        var existing = await db.AppleDeviceRegistrations
            .FirstOrDefaultAsync(r =>
                r.DeviceLibraryIdentifier == deviceId &&
                r.PassTypeIdentifier == passTypeId &&
                r.SerialNumber == serialNumber);

        if (existing != null)
        {
            // Atualizar push token
            if (!string.IsNullOrEmpty(pushToken))
                existing.PushToken = pushToken;
            await db.SaveChangesAsync();
            return Ok(); // 200 = já existia
        }

        db.AppleDeviceRegistrations.Add(new AppleDeviceRegistration
        {
            Id = Guid.NewGuid(),
            DeviceLibraryIdentifier = deviceId,
            PushToken = pushToken ?? string.Empty,
            PassTypeIdentifier = passTypeId,
            SerialNumber = serialNumber,
        });
        await db.SaveChangesAsync();

        return StatusCode(201); // 201 = registration criada
    }

    /// <summary>
    /// Desregistrar device de um pass.
    /// DELETE /v1/devices/{deviceId}/registrations/{passTypeId}/{serialNumber}
    /// </summary>
    [HttpDelete("devices/{deviceId}/registrations/{passTypeId}/{serialNumber}")]
    public async Task<IActionResult> UnregisterDevice(
        string deviceId, string passTypeId, string serialNumber)
    {
        var authToken = ExtractAuthToken();
        if (authToken == null) return Unauthorized();

        var enrollment = await db.Enrollments
            .FirstOrDefaultAsync(e =>
                e.ApplePassSerial == serialNumber &&
                e.ApplePassAuthToken == authToken);

        if (enrollment == null) return Unauthorized();

        var registration = await db.AppleDeviceRegistrations
            .FirstOrDefaultAsync(r =>
                r.DeviceLibraryIdentifier == deviceId &&
                r.PassTypeIdentifier == passTypeId &&
                r.SerialNumber == serialNumber);

        if (registration == null) return NotFound();

        db.AppleDeviceRegistrations.Remove(registration);
        await db.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Listar serial numbers de passes atualizados para um device.
    /// GET /v1/devices/{deviceId}/registrations/{passTypeId}?passesUpdatedSince=
    /// </summary>
    [HttpGet("devices/{deviceId}/registrations/{passTypeId}")]
    public async Task<IActionResult> GetSerialNumbers(
        string deviceId, string passTypeId, [FromQuery] string? passesUpdatedSince)
    {
        var registrations = await db.AppleDeviceRegistrations
            .Where(r =>
                r.DeviceLibraryIdentifier == deviceId &&
                r.PassTypeIdentifier == passTypeId)
            .Select(r => r.SerialNumber)
            .ToListAsync();

        if (registrations.Count == 0) return NoContent();

        // Em uma implementação completa, filtraríamos por data de atualização.
        // Por simplicidade, retornamos todos os serials registrados.
        return Ok(new
        {
            serialNumbers = registrations,
            lastUpdated = DateTime.UtcNow.ToString("o"),
        });
    }

    /// <summary>
    /// Retornar a versão mais recente de um pass (.pkpass).
    /// GET /v1/passes/{passTypeId}/{serialNumber}
    /// </summary>
    [HttpGet("passes/{passTypeId}/{serialNumber}")]
    public async Task<IActionResult> GetLatestPass(string passTypeId, string serialNumber)
    {
        var authToken = ExtractAuthToken();
        if (authToken == null) return Unauthorized();

        var enrollment = await db.Enrollments
            .Include(e => e.Cliente)
            .Include(e => e.Campanha)
                .ThenInclude(c => c.Negocio)
            .FirstOrDefaultAsync(e =>
                e.ApplePassSerial == serialNumber &&
                e.ApplePassAuthToken == authToken);

        if (enrollment == null) return Unauthorized();

        var passBytes = appleWallet.GeneratePass(
            enrollment, enrollment.Campanha, enrollment.Campanha.Negocio, enrollment.Cliente);

        if (passBytes == null) return StatusCode(500);

        Response.Headers["Last-Modified"] = DateTime.UtcNow.ToString("R");
        return File(passBytes, "application/vnd.apple.pkpass");
    }

    /// <summary>
    /// Receber logs de erro do device.
    /// POST /v1/log
    /// </summary>
    [HttpPost("log")]
    public IActionResult ReceiveLog()
    {
        // Apple envia logs de erros do dispositivo — apenas logar
        return Ok();
    }

    private string? ExtractAuthToken()
    {
        var auth = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(auth)) return null;

        // Formato: "ApplePass <token>"
        if (auth.StartsWith("ApplePass ", StringComparison.OrdinalIgnoreCase))
            return auth["ApplePass ".Length..];

        return null;
    }

    private record DeviceRegistrationBody(string? PushToken);
}
