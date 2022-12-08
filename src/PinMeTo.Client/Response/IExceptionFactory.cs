using System.Collections.Generic;
using System.Net;

namespace TheCodePatch.PinMeTo.Client.Response;

/// <summary>
/// Creates exceptions.
/// </summary>
internal interface IExceptionFactory
{
    /// <summary>
    /// Creates an appropriate exception to throw, based on the state of the specified faulted response.
    /// </summary>
    /// <param name="statusCode">The status code of the response.</param>
    /// <param name="responseContent">The content of the response</param>
    /// <returns>The appropriate exception.</returns>
    Exception CreateException(HttpStatusCode statusCode, string responseContent);
}
