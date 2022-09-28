using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record PendingChanges
{
    [JsonPropertyName("address")]
    public Address? Address { get; init; }

    [JsonPropertyName("location")]
    public Position? Position { get; init; }
}
