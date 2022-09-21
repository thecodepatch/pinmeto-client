using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Locations.Model;

public record Contact
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}
