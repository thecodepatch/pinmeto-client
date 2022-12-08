using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.Response;

/// <summary>
/// Handles a response from the PinMeTo API.
/// </summary>
internal interface IResponseHandler
{
    /// <summary>
    /// Deserializes the specified response to the specified model type.
    /// If the response is not successful, or if an error occurrs during
    /// deserialization, the appropriate exception is thrown.
    /// </summary>
    /// <param name="requestContent">The data sent in the request.</param>
    /// <param name="response">The response.</param>
    /// <param name="requestedUrl">The URL that the request was sent to.</param>
    /// <typeparam name="TModel">The type to deserialize to.</typeparam>
    /// <returns>The deserialized object.</returns>
    Task<TModel> DeserializeOrThrow<TModel>(
        string requestedUrl,
        object? requestContent,
        HttpResponseMessage response
    );
}
