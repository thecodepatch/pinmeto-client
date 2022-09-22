using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations.Model;

public record Facebook
{
    [JsonPropertyName("coverImage")]
    public string CoverImage { get; init; } = null!;

    [JsonPropertyName("link")]
    public string Link { get; init; } = null!;

    [JsonPropertyName("pageId")]
    public string PageId { get; init; } = null!;

    [JsonPropertyName("profileImage")]
    public string ProfileImage { get; init; } = null!;
}