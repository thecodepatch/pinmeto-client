using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record SpecialOpeningHours
{
    [JsonPropertyName("closeTime")]
    [Required]
    public TimeOnly? CloseTime { get; set; }

    [JsonPropertyName("end")]
    [Required]
    public DateOnly End { get; init; }

    [JsonPropertyName("isClosed")]
    [Required]
    [MemberNotNullWhen(false, nameof(CloseTime))]
    [MemberNotNullWhen(false, nameof(OpenTime))]
    public bool IsClosed { get; init; }

    [JsonPropertyName("label")]
    [Required]
    public string Label { get; init; } = null!;

    [JsonPropertyName("openTime")]
    [Required]
    public TimeOnly? OpenTime { get; set; }

    [JsonPropertyName("start")]
    [Required]
    public DateOnly Start { get; init; }
}
