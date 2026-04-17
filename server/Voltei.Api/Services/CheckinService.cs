using Microsoft.EntityFrameworkCore;
using Voltei.Api.Data;
using Voltei.Api.Dto;
using Voltei.Api.Models;
using Voltei.Api.Services.Wallet;

namespace Voltei.Api.Services;

public class CheckinService(AppDbContext db, GoogleWalletService googleWallet)
{
    public async Task<(CheckinResponse? response, string? error)> RegisterCheckinAsync(string token, Guid staffId)
    {
        var enrollment = await db.Enrollments
            .Include(e => e.Cliente)
            .Include(e => e.Campanha)
            .FirstOrDefaultAsync(e => e.Token == token);

        if (enrollment == null)
            return (null, "Token inválido — inscrição não encontrada.");

        if (!enrollment.Campanha.Ativa)
            return (null, "Esta campanha não está mais ativa.");

        if (enrollment.Resgatou)
            return (null, "Este cliente já resgatou o prêmio desta campanha.");

        if (enrollment.CheckinsAtuais >= enrollment.Campanha.CheckinsNecessarios)
            return (null, "Este cliente já completou todos os check-ins.");

        enrollment.CheckinsAtuais++;

        var log = new CheckinLog
        {
            Id = Guid.NewGuid(),
            ParticipacaoId = enrollment.Id,
            RegistradoPor = staffId,
        };
        db.CheckinLogs.Add(log);

        await db.SaveChangesAsync();

        var rewardReached = enrollment.CheckinsAtuais >= enrollment.Campanha.CheckinsNecessarios;

        // Atualizar Google Wallet (se configurado e se tem object)
        await googleWallet.UpdateLoyaltyObjectAsync(enrollment, enrollment.Campanha);

        return (new CheckinResponse
        {
            EnrollmentId = enrollment.Id.ToString(),
            ClienteNome = enrollment.Cliente.Nome,
            CampanhaNome = enrollment.Campanha.Nome,
            CampanhaDescricao = enrollment.Campanha.Descricao,
            CheckinsAtuais = enrollment.CheckinsAtuais,
            CheckinsNecessarios = enrollment.Campanha.CheckinsNecessarios,
            RewardReached = rewardReached,
        }, null);
    }

    public async Task<string?> RedeemAsync(Guid enrollmentId, Guid staffNegocioId)
    {
        var enrollment = await db.Enrollments
            .Include(e => e.Campanha)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);

        if (enrollment == null)
            return "Inscrição não encontrada.";

        if (enrollment.Campanha.NegocioId != staffNegocioId)
            return "Você não tem permissão para esta campanha.";

        if (enrollment.Resgatou)
            return "Prêmio já foi resgatado.";

        if (enrollment.CheckinsAtuais < enrollment.Campanha.CheckinsNecessarios)
            return "Cliente ainda não completou os check-ins necessários.";

        enrollment.Resgatou = true;
        await db.SaveChangesAsync();

        return null; // success
    }
}
