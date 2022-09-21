using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Locations.Model;

public record Description
{
    [JsonPropertyName("long")]
    [MaxLength(750)]
    public string? Long { get; init; }

    [JsonPropertyName("short")]
    [MaxLength(240)]
    public string? Short { get; init; }
}