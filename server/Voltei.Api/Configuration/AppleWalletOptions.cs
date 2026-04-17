namespace Voltei.Api.Configuration;

public class AppleWalletOptions
{
    public string PassTypeIdentifier { get; set; } = "pass.com.voltei.loyalty";
    public string TeamIdentifier { get; set; } = "VOLTEIDEV";
    public string CertificatePath { get; set; } = string.Empty;
    public string CertificatePassword { get; set; } = string.Empty;
    public string WebServiceUrl { get; set; } = string.Empty;
}
