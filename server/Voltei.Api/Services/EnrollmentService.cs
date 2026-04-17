using Microsoft.EntityFrameworkCore;
using Voltei.Api.Data;
using Voltei.Api.Dto;
using Voltei.Api.Models;
using Voltei.Api.Services.Wallet;

namespace Voltei.Api.Services;

public class EnrollmentService(
    AppDbContext db,
    GoogleWalletService googleWallet,
    AppleWalletService appleWallet)
{
    public async Task<(Campaign campaign, Business business)?> GetCampaignWithBusinessAsync(Guid campaignId)
    {
        var campaign = await db.Campaigns
            .Include(c => c.Negocio)
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.Ativa);

        if (campaign == null) return null;
        return (campaign, campaign.Negocio);
    }

    public async Task<Enrollment?> FindExistingEnrollmentAsync(Guid campaignId, string telefone)
    {
        return await db.Enrollments
            .Include(e => e.Cliente)
            .Include(e => e.Campanha)
            .FirstOrDefaultAsync(e =>
                e.CampanhaId == campaignId &&
                e.Cliente.Telefone == telefone);
    }

    public async Task<(Enrollment enrollment, bool alreadyEnrolled, string? googleWalletSaveUrl)> EnrollAsync(
        Guid campaignId, EnrollRequest request)
    {
        // Find or create customer
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.Telefone == request.Telefone);

        if (customer == null)
        {
            customer = new Customer
            {
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                Telefone = request.Telefone,
            };
            db.Customers.Add(customer);
        }

        // Check if already enrolled
        var existing = await db.Enrollments
            .Include(e => e.Campanha)
                .ThenInclude(c => c.Negocio)
            .FirstOrDefaultAsync(e =>
                e.CampanhaId == campaignId &&
                e.ClienteId == customer.Id);

        if (existing != null)
        {
            existing.Cliente = customer;
            return (existing, true, null);
        }

        // Create enrollment
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            CampanhaId = campaignId,
            ClienteId = customer.Id,
        };

        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        // Load navigation properties
        var campaign = await db.Campaigns
            .Include(c => c.Negocio)
            .FirstAsync(c => c.Id == campaignId);
        enrollment.Cliente = customer;
        enrollment.Campanha = campaign;

        // Google Wallet — gerar save URL
        string? googleSaveUrl = null;
        if (googleWallet.IsConfigured)
        {
            googleSaveUrl = await googleWallet.CreateLoyaltyObjectAsync(
                enrollment, campaign, campaign.Negocio, customer);

            if (googleSaveUrl != null)
            {
                enrollment.WalletObjectId = $"{enrollment.Id}";
                await db.SaveChangesAsync();
            }
        }

        // Apple Wallet — gerar .pkpass e salvar serial/auth token
        if (appleWallet.IsConfigured)
        {
            enrollment.ApplePassSerial = Guid.NewGuid().ToString("N");
            enrollment.ApplePassAuthToken = Convert.ToHexStringLower(
                System.Security.Cryptography.RandomNumberGenerator.GetBytes(16));

            var passBytes = appleWallet.GeneratePass(enrollment, campaign, campaign.Negocio, customer);
            if (passBytes != null)
            {
                await db.SaveChangesAsync();
            }
            else
            {
                // Reset se não gerou
                enrollment.ApplePassSerial = null;
                enrollment.ApplePassAuthToken = null;
            }
        }

        return (enrollment, false, googleSaveUrl);
    }
}
