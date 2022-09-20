using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Response;

internal record PagedResponse<TItems>
{
    [JsonPropertyName("data")]
    public List<TItems> Data { get; init; } = null!;

    [JsonPropertyName("paging")]
    public PagingModel Paging { get; init; } = null!;

    internal record PagingModel
    {
        [JsonPropertyName("before")]
        public string Before { get; init; } = null!;

        [JsonPropertyName("next")]
        public string Next { get; init; } = null!;
    }
}
