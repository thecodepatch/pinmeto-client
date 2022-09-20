using System.Net.Http.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TheCodePatch.PinMeToClient.Exceptions;

namespace TheCodePatch.PinMeToClient.Response;

internal class ResponseHandler : IResponseHandler
{
    private readonly ILogger<ResponseHandler> _logger;

    public ResponseHandler(ILogger<ResponseHandler> logger)
    {
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
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug(
                "An OK response was retrieved from the API: {ResponseContent}",
                responseContent
            );
        }

        var deserialized = await response.Content.ReadFromJsonAsync<TModel>();
        if (deserialized == null)
        {
            throw new ResponseFormatException("The response could not be deserialized.");
        }

        return deserialized;
    }

    private async Task<Exception> ThrowFailureException(HttpResponseMessage response)
    {
        var content = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        _logger.LogError("An error was retrieved from the API: {Error}", content);
        return new PinMeToException(content?.Description ?? "Unexpected error");
    }
}
