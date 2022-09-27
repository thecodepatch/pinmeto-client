using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record LocationDetails<TCustomData> : LocationBase<TCustomData>
{
    [JsonPropertyName("network")]
    public NetworkModel Network { get; set; } = null!;

    [JsonPropertyName("facebookName")]
    public string? FacebookName { get; set; }

    [JsonPropertyName("googleName")]
    public string? GoogleName { get; set; }

    [JsonPropertyName("isAlwaysOpen")]
    public bool IsAlwaysOpen { get; set; }

    [JsonPropertyName("pendingChanges")]
    public PendingChanges PendingChanges { get; set; } = null!;

    [JsonPropertyName("permanentlyClosed")]
    public bool IsPermanentlyClosed { get; set; }

    [JsonPropertyName("temporarilyClosedUntil")]
    public DateOnly? IsTemporarilyClosedUntil { get; set; }

    [JsonPropertyName("wifiSsid")]
    public string? WifiSsid { get; set; }

    public record NetworkModel
    {
        [JsonPropertyName("facebook")]
        public Facebook Facebook { get; set; } = null!;

        [JsonPropertyName("google")]
        public Google Google { get; set; } = null!;
    }
}
