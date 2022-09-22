using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record OpenHours
{
    [JsonPropertyName("mon")]
    [Required]
    public OpenHoursDay Monday { get; init; } = new();

    [JsonPropertyName("tue")]
    [Required]
    public OpenHoursDay Tuesday { get; init; } = new();

    [JsonPropertyName("wed")]
    [Required]
    public OpenHoursDay Wednesday { get; init; } = new();

    [JsonPropertyName("thu")]
    [Required]
    public OpenHoursDay Thursday { get; init; } = new();

    [JsonPropertyName("fri")]
    [Required]
    public OpenHoursDay Friday { get; init; } = new();

    [JsonPropertyName("sat")]
    [Required]
    public OpenHoursDay Saturday { get; init; } = new();

    [JsonPropertyName("sun")]
    [Required]
    public OpenHoursDay Sunday { get; init; } = new();

    public record OpenHoursDay
    {
        [JsonPropertyName("span")]
        [Required]
        public List<OpenHoursTimeSpan> Times { get; set; } = new();

        [JsonPropertyName("state")]
        [Required]
        public OpenState State { get; set; }

        public enum OpenState
        {
            NotSpecified,
            Closed,
            Open,
            AlwaysOpen,
        }

        public record OpenHoursTimeSpan
        {
            [JsonPropertyName("close")]
            [Required]
            public TimeOnly Closes { get; init; }

            [JsonPropertyName("open")]
            [Required]
            public TimeOnly Opens { get; init; }
        }
    }
}
