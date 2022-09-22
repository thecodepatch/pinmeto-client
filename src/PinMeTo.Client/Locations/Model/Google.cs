using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record Google
{
    [JsonPropertyName("coverImage")]
    public string CoverImage { get; init; } = null!;

    [JsonPropertyName("link")]
    public string Link { get; init; } = null!;

    [JsonPropertyName("newReviewUrl")]
    public string NewReviewUrl { get; init; } = null!;

    [JsonPropertyName("placeId")]
    public string PlaceId { get; init; } = null!;

    [JsonPropertyName("profileImage")]
    public string ProfileImage { get; init; } = null!;
}