using System.ComponentModel.DataAnnotations;

namespace Voltei.Api.Dto;

public class SignupRequest
{
    [Required] public string NomeNegocio { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, MinLength(6)] public string Senha { get; set; } = string.Empty;
}
