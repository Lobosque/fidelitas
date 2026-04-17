using System.ComponentModel.DataAnnotations;

namespace Voltei.Api.Dto;

public class EnrollRequest
{
    [Required] public string Nome { get; set; } = string.Empty;
    [Required, Phone] public string Telefone { get; set; } = string.Empty;
}
