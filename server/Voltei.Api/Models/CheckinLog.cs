namespace Voltei.Api.Models;

public class CheckinLog
{
    public Guid Id { get; set; }
    public Guid ParticipacaoId { get; set; }
    public Guid RegistradoPor { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Enrollment Participacao { get; set; } = null!;
    public User Staff { get; set; } = null!;
}
