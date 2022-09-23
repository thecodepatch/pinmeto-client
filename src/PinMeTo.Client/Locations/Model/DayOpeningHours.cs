using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record DayOpeningHours
{
    [JsonPropertyName("span")]
    [Required]
    public List<OpeningHours> Times { get; set; } = new();

    [JsonPropertyName("state")]
    [Required]
    public OpenState State { get; set; }
}