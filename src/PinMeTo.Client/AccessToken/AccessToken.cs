using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.AccessToken;

/// <summary>
///     An existing access token.
/// </summary>
internal record AccessToken
{
    /// <summary>
    ///     The date and time of the expiry of this access token.
    /// </summary>
    public DateTime Expires { get; init; }

    /// <summary>
    ///     Gets the amount of seconds that remains of the validity of this access token.
    /// </summary>
    /// <returns></returns>
    public double GetRemainingValiditySeconds()
    {
        return (Expires - DateTime.Now).TotalSeconds;
    }

    /// <summary>
    ///     The access token value.
    /// </summary>
    public string Value { get; init; } = null!;
}
