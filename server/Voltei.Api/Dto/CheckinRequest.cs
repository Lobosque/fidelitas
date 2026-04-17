using System.ComponentModel.DataAnnotations;

namespace Voltei.Api.Dto;

public class CheckinRequest
{
    [Required] public string EnrollmentToken { get; set; } = string.Empty;
}
