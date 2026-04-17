namespace Voltei.Api.Models;

public enum Plano
{
    Gratis,
    Profissional,
    Negocio
}

public class Business
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string CoresPrimaria { get; set; } = "#4f46e5";
    public string CoresSecundaria { get; set; } = "#818cf8";
    public Plano Plano { get; set; } = Plano.Gratis;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<User> Users { get; set; } = [];
    public ICollection<Campaign> Campaigns { get; set; } = [];
}
