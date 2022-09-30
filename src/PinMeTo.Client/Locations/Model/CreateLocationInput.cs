using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record CreateLocationInput<TCustomData> : LocationBase<TCustomData>
{
    public CreateLocationInput()
    {
        // Position must not be null when submitting to the API.
        this.Position = new();
    }

    [JsonPropertyName("permanentlyClosed")]
    public bool? IsPermanentlyClosed { get; set; }

    [JsonPropertyName("temporarilyClosedUntil")]
    public DateOnly? IsTemporarilyClosedUntil { get; set; }

    [JsonPropertyName("isAlwaysOpen")]
    public bool? IsAlwaysOpen { get; set; }

    [JsonPropertyName("facebookName")]
    public string? FacebookName { get; set; }

    [JsonPropertyName("wifiSsid")]
    [MaxLength(32)]
    public string? WifiSsid { get; set; }

    [JsonPropertyName("googleName")]
    public string? GoogleName { get; set; }
}
