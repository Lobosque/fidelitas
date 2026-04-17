namespace Voltei.Api.Models;

public class AppleDeviceRegistration
{
    public Guid Id { get; set; }
    public string DeviceLibraryIdentifier { get; set; } = string.Empty;
    public string PushToken { get; set; } = string.Empty;
    public string PassTypeIdentifier { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
