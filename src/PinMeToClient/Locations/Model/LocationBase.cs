﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Locations.Model;

public abstract record LocationBase
{
    [JsonPropertyName("storeId")]
    [Required]
    public string StoreId { get; init; } = null!;

    [JsonPropertyName("address")]
    [Required]
    public Address Address { get; init; } = new();

    [JsonPropertyName("specialOpenHours")]
    public List<SpecialOpenHour>? SpecialOpenHours { get; init; }

    [JsonPropertyName("contact")]
    [Required]
    public Contact Contact { get; init; } = new();

    [JsonPropertyName("description")]
    public Description? Description { get; init; }

    [JsonPropertyName("name")]
    [Required]
    public string Name { get; init; } = null!;

    [JsonPropertyName("locationDescriptor")]
    public string? LocationDescriptor { get; init; }

    [JsonPropertyName("openHours")]
    public OpenHours? OpenHours { get; set; }

    [JsonPropertyName("location")]
    [Required]
    public Position Position { get; init; } = new();
}
