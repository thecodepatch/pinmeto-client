using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record SpecialOpeningHours
{
    [JsonPropertyName("closeTime")] // TODO or endTime?
    [Required]
    public TimeOnly CloseTime { get; init; }

    [JsonPropertyName("end")]
    [Required]
    public DateOnly End { get; init; }

    [JsonPropertyName("isClosed")]
    [Required]
    public bool IsClosed { get; init; }

    [JsonPropertyName("label")]
    [Required]
    public string Label { get; init; } = null!;

    [JsonPropertyName("openTime")]
    [Required]
    public TimeOnly OpenTime { get; init; }

    [JsonPropertyName("start")]
    [Required]
    public DateOnly Start { get; init; }
}
