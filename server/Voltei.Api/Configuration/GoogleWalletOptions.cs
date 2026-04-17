namespace Voltei.Api.Configuration;

public class GoogleWalletOptions
{
    public string IssuerId { get; set; } = string.Empty;
    public string ServiceAccountEmail { get; set; } = string.Empty;
    public string ServiceAccountPrivateKey { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = "Voltei";
}
