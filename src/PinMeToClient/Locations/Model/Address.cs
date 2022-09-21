using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Locations.Model;

public record Address
{
    [JsonPropertyName("city")]
    [Required]
    public string City { get; set; } = null!;

    [JsonPropertyName("country")]
    [Required]
    public string Country { get; set; } = null!;

    [JsonPropertyName("street")]
    [Required]
    public string Street { get; set; } = null!;

    [JsonPropertyName("zip")]
    [Required]
    public string Zip { get; set; } = null!;

    [JsonPropertyName("state")]
    public string? State { get; set; }
}
