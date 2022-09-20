using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeToClient.Response;

internal record AtomicResponse<TItem>
{
    [JsonPropertyName("data")]
    public TItem Data { get; init; } = default!;
}
