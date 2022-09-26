using System.Collections.Generic;
using System.Collections.Specialized;

namespace TheCodePatch.PinMeTo.Client.Clients;

internal interface IUrlFactory
{
    /// <summary>
    /// Creates a relative url for use with a resource that requires an account ID.
    /// </summary>
    /// <param name="relativePath">The relative path to the resource.</param>
    /// <param name="queryParameters">Any query parameters</param>
    /// <returns>The full, relative url.</returns>
    string CreateRelativeUrl(
        string relativePath,
        params (string name, string value)[] queryParameters
    );
}
