namespace Voltei.Api.Models;

public class Campaign
{
    public Guid Id { get; set; }
    public Guid NegocioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int CheckinsNecessarios { get; set; }
    public bool Ativa { get; set; } = true;
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    public string? WalletClassId { get; set; }
    public string? ApplePassTypeId { get; set; }

    public Business Negocio { get; set; } = null!;
    public ICollection<Enrollment> Enrollments { get; set; } = [];
}
