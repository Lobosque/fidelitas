namespace Voltei.Api.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public Guid NegocioId { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Business Negocio { get; set; } = null!;
}
