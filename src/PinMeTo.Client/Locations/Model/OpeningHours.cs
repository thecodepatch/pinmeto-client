using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record OpeningHours
{
    [JsonPropertyName("close")]
    [Required]
    public TimeOnly Closes { get; init; }

    [JsonPropertyName("open")]
    [Required]
    public TimeOnly Opens { get; init; }
}