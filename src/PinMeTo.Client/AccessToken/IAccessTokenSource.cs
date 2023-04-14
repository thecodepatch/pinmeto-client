using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.AccessToken;

/// <summary>
///     Handles retrieval and storage of the access token required to communicate with the PinMeTo API.
/// </summary>
// ReSharper disable once UnusedTypeParameter
internal interface IAccessTokenSource<TCustomData> // The type parameter here is to distinguish token sources depending on the type of custom data.
{
    /// <summary>
    ///     Gets an access token that should be used when communicating with the PinMeTo API.
    /// </summary>
    /// <returns>The access token.</returns>
    Task<string> GetAccessToken();
}
