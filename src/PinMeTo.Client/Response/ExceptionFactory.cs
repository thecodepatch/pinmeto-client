using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.Logging;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Serialization;
using Exception = System.Exception;

namespace TheCodePatch.PinMeTo.Client.Response;

internal class ExceptionFactory : IExceptionFactory
{
    private readonly ISerializer _serializer;
    private readonly ILogger<ExceptionFactory> _logger;

    public ExceptionFactory(ISerializer serializer, ILogger<ExceptionFactory> logger)
    {
        _serializer = serializer;
        _logger = logger;
    }

    public async Task<Exception> CreateException(HttpResponseMessage faultedResponse)
    {
        var responseContent = await faultedResponse.Content.ReadAsStringAsync();
        var appropriateException = faultedResponse.StatusCode switch
        {
            HttpStatusCode.NotFound => NotFound(responseContent),
            HttpStatusCode.BadRequest => BadRequest(responseContent),
            HttpStatusCode.UnprocessableEntity => UnprocessableEntity(responseContent),
            _ => null,
        };

        if (null != appropriateException)
        {
            return appropriateException;
        }

        _logger.LogError(
            "Unexpected error with status code {StatusCode} on {Method} request to {Url}: {ResponseContent}",
            faultedResponse.StatusCode,
            faultedResponse.RequestMessage?.Method,
            faultedResponse.RequestMessage?.RequestUri,
            responseContent
        );

        return new PinMeToException("Unexpected bad request");
    }

    private Exception NotFound(string responseContent)
    {
        _logger.LogDebug("NotFound returned from API: {ResponseContent}", responseContent);
        return new NotFoundException(responseContent);
    }

    private Exception? BadRequest(string responseContent)
    {
        if (TryDeserializeErrorModel(out var exceptionBasedOnErrorModel))
        {
            return exceptionBasedOnErrorModel;
        }

        if (TryDeserializeValidationErrorsInAtomicResponse(out var validationException1))
        {
            return validationException1;
        }

        if (TryDeserializeValidationErrors(out var validationException2))
        {
            return validationException2;
        }

        return null;

        bool TryDeserializeErrorModel([NotNullWhen(true)] out Exception? e)
        {
            var errorModel = _serializer.Deserialize<ErrorResponse>(responseContent);
            if (errorModel.Error == "invalid_request")
            {
                _logger.LogError(
                    "Invalid Request returned from API: {ResponseContent}",
                    responseContent
                );
                e = new InvalidPinMeToRequestException(errorModel.Description);
                return true;
            }

            e = null;
            return false;
        }

        bool TryDeserializeValidationErrorsInAtomicResponse(
            [NotNullWhen(true)] out Exception? exception
        )
        {
            var validationErrors = _serializer.Deserialize<
                AtomicResponse<Dictionary<string, List<string>>>
            >(responseContent);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (null != validationErrors.Data)
            {
                _logger.LogWarning(
                    "Validation errors 1: {ValidationErrors}",
                    _serializer.Serialize(validationErrors.Data)
                );
                exception = new ValidationErrorsException(validationErrors.Data);
                return true;
            }

            exception = null;
            return false;
        }

        bool TryDeserializeValidationErrors([NotNullWhen(true)] out Exception? e)
        {
            var validationErrors2 = _serializer.Deserialize<Dictionary<string, List<string>>>(
                responseContent
            );
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (null != validationErrors2)
            {
                _logger.LogWarning(
                    "Validation errors 2: {ValidationErrors}",
                    _serializer.Serialize(validationErrors2)
                );
                e = new ValidationErrorsException(validationErrors2);
                return true;
            }

            e = null;
            return false;
        }
    }

    static Exception UnprocessableEntity(string responseContent)
    {
        return new MissingRequiredPropertyException(responseContent);
    }
}
