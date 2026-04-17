namespace Voltei.Api.Models;

public class Enrollment
{
    public Guid Id { get; set; }
    public Guid CampanhaId { get; set; }
    public Guid ClienteId { get; set; }
    public int CheckinsAtuais { get; set; }
    public bool Resgatou { get; set; }
    public string Token { get; set; } = Guid.NewGuid().ToString("N");

    // Google Wallet
    public string? WalletObjectId { get; set; }

    // Apple Wallet
    public string? ApplePassSerial { get; set; }
    public string? ApplePassAuthToken { get; set; }
    public string? ApplePushToken { get; set; }

    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public Campaign Campanha { get; set; } = null!;
    public Customer Cliente { get; set; } = null!;
    public ICollection<CheckinLog> CheckinLogs { get; set; } = [];
}
