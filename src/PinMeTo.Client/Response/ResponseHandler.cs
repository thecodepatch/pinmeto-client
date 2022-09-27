using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.Extensions.Logging;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Serialization;

namespace TheCodePatch.PinMeTo.Client.Response;

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
        return response.StatusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.BadRequest => BadRequest(),
            HttpStatusCode.UnprocessableEntity => UnprocessableEntity(),
            _ => Unexpected(),
        };

        Exception NotFound()
        {
            _logger.LogError("NotFound returned from API: {ResponseContent}", responseContent);
            return new NotFoundException(responseContent);
        }
        Exception BadRequest()
        {
            var errorModel = _serializer.Deserialize<ErrorResponse>(responseContent);
            if (errorModel.Error == "invalid_request")
            {
                _logger.LogError(
                    "Invalid Request returned from API: {ResponseContent}",
                    responseContent
                );
                return new InvalidPinMeToRequestException(errorModel.Description);
            }

            try
            {
                var validationErrors = _serializer.Deserialize<
                    AtomicResponse<Dictionary<string, List<string>>>
                >(responseContent);
                return new ValidationErrorsException(validationErrors.Data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize validation errors");
            }

            _logger.LogError(
                "Unexpected BadRequest returned from API: {ResponseContent}",
                responseContent
            );
            return new PinMeToException("Unexpected bad request");
        }

        Exception UnprocessableEntity()
        {
            return new MissingRequiredPropertyException(responseContent);
        }
        Exception Unexpected()
        {
            _logger.LogError(
                "An unexpected error was retrieved from the API {StatusCode}: {ResponseContent}",
                response.StatusCode,
                responseContent
            );
            return new PinMeToException(responseContent);
        }
    }
}
