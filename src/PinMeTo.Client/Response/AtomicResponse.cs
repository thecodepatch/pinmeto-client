using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Response;

internal record AtomicResponse<TItem>
{
    [JsonPropertyName("data")]
    public TItem Data { get; init; } = default!;
}
