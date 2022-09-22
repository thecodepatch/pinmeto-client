using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record SpecialOpeningHour
{
    [JsonPropertyName("closeTime")] // TODO or endTime?
    [Required]
    public TimeOnly CloseTime { get; init; } = default!;

    [JsonPropertyName("end")]
    [Required]
    public DateOnly End { get; init; } = default!;

    [JsonPropertyName("isClosed")]
    [Required]
    public bool IsClosed { get; init; }

    [JsonPropertyName("label")]
    [Required]
    public string Label { get; init; } = null!;

    [JsonPropertyName("openTime")]
    [Required]
    public TimeOnly OpenTime { get; init; } = default!;

    [JsonPropertyName("start")]
    [Required]
    public DateOnly Start { get; init; } = default!;
}