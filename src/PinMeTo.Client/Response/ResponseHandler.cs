using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Serialization;

namespace TheCodePatch.PinMeTo.Client.Response;

internal class ResponseHandler(
    ISerializer serializer,
    IExceptionFactory exceptionFactory,
    ILogger<ResponseHandler> logger)
    : IResponseHandler
{
    /// <inheritdoc />
    public async Task<TModel> DeserializeOrThrow<TModel>(
        string url,
        object? requestContent,
        HttpResponseMessage response
    )
    {
        if (response.IsSuccessStatusCode)
        {
            return await HandleSuccess<TModel>(response);
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var exception = exceptionFactory.CreateException(response.StatusCode, responseContent);
        if (exception is PinMeToException)
        {
            logger.LogError(
                "Unexpected error with status code {StatusCode} on {Method} request to {Url} with request content {@RequestContent} Response: {ResponseContent}",
                response.StatusCode,
                response.RequestMessage?.Method,
                response.RequestMessage?.RequestUri,
                requestContent,
                responseContent
            );
        }

        throw exception;
    }

    private async Task<TModel> HandleSuccess<TModel>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        logger.LogDebug(
            "An OK response was retrieved from the API: {ResponseContent}",
            responseContent
        );
        var deserialized = serializer.Deserialize<TModel>(responseContent);
        return deserialized;
    }
}
