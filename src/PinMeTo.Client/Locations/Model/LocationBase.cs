﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public abstract record LocationBase<TCustomData>
{
    [JsonPropertyName("storeId")]
    [Required]
    public string StoreId { get; init; } = null!;

    [JsonPropertyName("address")]
    [Required]
    public Address Address { get; init; } = new();

    [JsonPropertyName("specialOpenHours")]
    public List<SpecialOpeningHours> SpecialOpeningHours { get; init; } = new();

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
    public WeekOpeningHours OpeningHours { get; set; } = new();

    [JsonPropertyName("location")]
    [Required]
    public Position? Position { get; set; }

    [JsonPropertyName("customData")]
    public TCustomData? CustomData { get; init; }
}
