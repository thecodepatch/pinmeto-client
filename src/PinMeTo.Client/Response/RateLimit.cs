namespace TheCodePatch.PinMeTo.Client.Response;

/// <summary>
/// Represents the rate limit.
/// </summary>
/// <param name="Limit">The hourly limit of requests.</param>
/// <param name="Reset">TODO - what does this mean?</param>
/// <param name="Remaining">The remaining quota.</param>
public record RateLimit(int Limit, int? Reset, int Remaining);
