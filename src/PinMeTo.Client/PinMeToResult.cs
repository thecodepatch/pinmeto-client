using System.Collections.Generic;
using TheCodePatch.PinMeTo.Client.Response;

namespace TheCodePatch.PinMeTo.Client;

/// <summary>
/// Generic result from the API that include rate limit info (if available)
/// and the actual result.
/// </summary>
/// <typeparam name="TResult">The type of the contained result.</typeparam>
public record PinMeToResult<TResult>
{
    /// <summary>
    /// Rate limit information.
    /// </summary>
    public RateLimit? RateLimit { get; init; }

    /// <summary>
    /// The result data.
    /// </summary>
    public TResult Result { get; init; } = default!;
}
