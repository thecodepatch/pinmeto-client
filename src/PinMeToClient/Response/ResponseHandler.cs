using System.Net.Http.Json;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using TheCodePatch.PinMeToClient.Exceptions;
using TheCodePatch.PinMeToClient.Serialization;

namespace TheCodePatch.PinMeToClient.Response;

internal class ResponseHandler : IResponseHandler
{
    private readonly ISerializer _serializer;
    private readonly ILogger<ResponseHandler> _logger;

    public ResponseHandler(ISerializer serializer, ILogger<ResponseHandler> logger)
    {
        _serializer = serializer;
        _logger = logger;
    }

    public async Task<TModel> DeserializeOrThrow<TModel>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return await HandleSuccess<TModel>(response);
        }

        throw await ThrowFailureException(response);
    }

    private async Task<TModel> HandleSuccess<TModel>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        _logger.LogDebug(
            "An OK response was retrieved from the API: {ResponseContent}",
            responseContent
        );
        var deserialized = _serializer.Deserialize<TModel>(responseContent);
        return deserialized;
    }

    private async Task<Exception> ThrowFailureException(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        _logger.LogError("An error was retrieved from the API: {Error}", responseContent);
        var errorModel = _serializer.Deserialize<ErrorResponse>(responseContent);
        return new PinMeToException(errorModel.Description);
    }
}
