namespace TheCodePatch.PinMeTo.Client.Response;

/// <summary>
/// Represents the rate limit.
/// </summary>
/// <param name="Limit">The hourly limit of requests.</param>
/// <param name="WillBeReset">When the limit will be reset.</param>
/// <param name="Remaining">The remaining quota.</param>
public record RateLimit(int Limit, DateTime WillBeReset, int Remaining);
