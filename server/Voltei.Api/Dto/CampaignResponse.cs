namespace Voltei.Api.Dto;

public class CampaignResponse
{
    public string Id { get; set; } = string.Empty;
    public string NegocioId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int CheckinsNecessarios { get; set; }
    public bool Ativa { get; set; }
    public string CriadaEm { get; set; } = string.Empty;
    public string? WalletClassId { get; set; }
}
