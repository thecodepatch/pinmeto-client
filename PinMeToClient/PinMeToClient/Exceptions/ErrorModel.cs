using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Exceptions;

/// <summary>
/// Model for the error messages returned by the API.
/// </summary>
internal record ErrorModel
{
    [JsonPropertyName("code")]
    public int ErrorCode { get; init; }

    [JsonPropertyName("error")]
    public string Name { get; init; } = null!;

    [JsonPropertyName("error_description")]
    public string Description { get; init; } = null!;
}
