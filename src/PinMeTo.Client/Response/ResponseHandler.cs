using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Serialization;

namespace TheCodePatch.PinMeTo.Client.Response;

internal class ResponseHandler : IResponseHandler
{
    private readonly ISerializer _serializer;
    private readonly IExceptionFactory _exceptionFactory;
    private readonly ILogger<ResponseHandler> _logger;

    public ResponseHandler(
        ISerializer serializer,
        IExceptionFactory exceptionFactory,
        ILogger<ResponseHandler> logger
    )
    {
        _serializer = serializer;
        _exceptionFactory = exceptionFactory;
        _logger = logger;
    }

    public async Task<TModel> DeserializeOrThrow<TModel>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return await HandleSuccess<TModel>(response);
        }

        throw await _exceptionFactory.CreateException(response);
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
}
