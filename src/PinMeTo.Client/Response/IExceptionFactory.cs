using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.Response;

/// <summary>
/// Creates exceptions.
/// </summary>
internal interface IExceptionFactory
{
    /// <summary>
    /// Creates an appropriate exception to throw, based on the state of the specified faulted response.
    /// </summary>
    /// <param name="faultedResponse">A response that has information about some kind of error.</param>
    /// <returns>The appropriate exception.</returns>
    Task<Exception> CreateException(HttpResponseMessage faultedResponse);
}
