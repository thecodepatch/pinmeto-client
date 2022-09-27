using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

/// <summary>
/// Properties to update in a location. Only root properties that have a value will be updated.
/// There are no required properties for updating a location. However, if you want to update a
/// field like 'Address.Street' or 'OpeningHours.Thu' you have to include all fields in that property,
/// not just the field you want to update.
/// </summary>
public record UpdateLocationInput
{
    [JsonPropertyName("description")]
    public Description? Description { get; set; }

    [JsonPropertyName("contact")]
    public Contact? Contact { get; set; }

    [JsonPropertyName("address")]
    public Address? Address { get; set; } = null!;

    [JsonPropertyName("permanentlyClosed")]
    public bool? IsPermanentlyClosed { get; set; }

    [JsonPropertyName("temporarilyClosedUntil")]
    public DateOnly? IsTemporarilyClosedUntil { get; set; }

    [JsonPropertyName("isAlwaysOpen")]
    public bool? IsAlwaysOpen { get; set; }

    [JsonPropertyName("locationDescriptor")]
    public string? LocationDescriptor { get; set; }

    [JsonPropertyName("location")]
    public Position? Position { get; set; }

    [JsonPropertyName("openHours")]
    public WeekOpeningHours? OpeningHours { get; set; }

    [JsonPropertyName("specialOpenHours")]
    public List<SpecialOpeningHours>? SpecialOpeningHours { get; set; }

    [JsonPropertyName("googleName")]
    public string? GoogleName { get; set; }

    [JsonPropertyName("facebookName")]
    public string? FacebookName { get; set; }

    [JsonPropertyName("wifiSsid")]
    [MaxLength(32)]
    public string? WifiSsid { get; set; }

    [JsonPropertyName("customData")]
    public Dictionary<string, object>? CustomData { get; set; }
}
