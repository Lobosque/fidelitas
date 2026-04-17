namespace Voltei.Api.Dto;

public class CheckinResponse
{
    public string EnrollmentId { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
    public string CampanhaNome { get; set; } = string.Empty;
    public string CampanhaDescricao { get; set; } = string.Empty;
    public int CheckinsAtuais { get; set; }
    public int CheckinsNecessarios { get; set; }
    public bool RewardReached { get; set; }
}
