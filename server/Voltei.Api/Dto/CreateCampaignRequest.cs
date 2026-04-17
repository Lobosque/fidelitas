using System.ComponentModel.DataAnnotations;

namespace Voltei.Api.Dto;

public class CreateCampaignRequest
{
    [Required] public string Nome { get; set; } = string.Empty;
    [Required] public string Descricao { get; set; } = string.Empty;
    [Range(2, 50)] public int CheckinsNecessarios { get; set; }
}
