namespace Voltei.Api.Dto;

public class EnrollResponse
{
    public string EnrollmentId { get; set; } = string.Empty;
    public string? GoogleWalletSaveUrl { get; set; }
    public string? ApplePassUrl { get; set; }
    public int CheckinsAtuais { get; set; }
    public int CheckinsNecessarios { get; set; }
    public bool AlreadyEnrolled { get; set; }
}
