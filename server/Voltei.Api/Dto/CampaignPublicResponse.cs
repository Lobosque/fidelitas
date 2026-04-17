namespace Voltei.Api.Dto;

public class CampaignPublicResponse
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int CheckinsNecessarios { get; set; }
    public string NegocioNome { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string CoresPrimaria { get; set; } = string.Empty;
    public string CoresSecundaria { get; set; } = string.Empty;
}
