using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Locations.Model;

public record Position
{
    [JsonPropertyName("lat")]
    [Required]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    [Required]
    public double Longitude { get; set; }
}
