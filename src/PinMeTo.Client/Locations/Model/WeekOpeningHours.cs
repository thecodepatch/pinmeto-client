using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record WeekOpeningHours
{
    [JsonPropertyName("mon")]
    [Required]
    public DayOpeningHours Monday { get; init; } = new();

    [JsonPropertyName("tue")]
    [Required]
    public DayOpeningHours Tuesday { get; init; } = new();

    [JsonPropertyName("wed")]
    [Required]
    public DayOpeningHours Wednesday { get; init; } = new();

    [JsonPropertyName("thu")]
    [Required]
    public DayOpeningHours Thursday { get; init; } = new();

    [JsonPropertyName("fri")]
    [Required]
    public DayOpeningHours Friday { get; init; } = new();

    [JsonPropertyName("sat")]
    [Required]
    public DayOpeningHours Saturday { get; init; } = new();

    [JsonPropertyName("sun")]
    [Required]
    public DayOpeningHours Sunday { get; init; } = new();
}
