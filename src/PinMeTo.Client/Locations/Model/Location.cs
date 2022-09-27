using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record Location<TCustomData> : LocationBase<TCustomData>
{
    [JsonPropertyName("network")]
    public NetworkModel Network { get; init; } = null!;

    [JsonPropertyName("openingDate")]
    public string? OpeningDate { get; init; }

    public record NetworkModel
    {
        [JsonPropertyName("facebook")]
        public Facebook Facebook { get; init; } = null!;

        [JsonPropertyName("google")]
        public Google Google { get; init; } = null!;

        [JsonPropertyName("pendingChanges")]
        public PendingChanges PendingChanges { get; init; } = null!;

        [JsonPropertyName("wifiSsid")]
        public string WifiSsid { get; init; } = null!;
    }
}
