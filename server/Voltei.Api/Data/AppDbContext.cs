using Microsoft.EntityFrameworkCore;
using Voltei.Api.Models;

namespace Voltei.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<CheckinLog> CheckinLogs => Set<CheckinLog>();
    public DbSet<AppleDeviceRegistration> AppleDeviceRegistrations => Set<AppleDeviceRegistration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Negocio)
            .WithMany(b => b.Users)
            .HasForeignKey(u => u.NegocioId);

        modelBuilder.Entity<Campaign>()
            .HasOne(c => c.Negocio)
            .WithMany(b => b.Campaigns)
            .HasForeignKey(c => c.NegocioId);

        // Customer
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Telefone)
            .IsUnique();

        // Enrollment
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.CampanhaId, e.ClienteId })
            .IsUnique();

        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => e.Token)
            .IsUnique();

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Campanha)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CampanhaId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Cliente)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.ClienteId);

        // CheckinLog
        modelBuilder.Entity<CheckinLog>()
            .HasOne(cl => cl.Participacao)
            .WithMany(e => e.CheckinLogs)
            .HasForeignKey(cl => cl.ParticipacaoId);

        modelBuilder.Entity<CheckinLog>()
            .HasOne(cl => cl.Staff)
            .WithMany()
            .HasForeignKey(cl => cl.RegistradoPor);

        // AppleDeviceRegistration
        modelBuilder.Entity<AppleDeviceRegistration>()
            .HasIndex(r => new { r.DeviceLibraryIdentifier, r.PassTypeIdentifier, r.SerialNumber })
            .IsUnique();
    }
}
