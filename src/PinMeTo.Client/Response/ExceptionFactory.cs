using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.Logging;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Serialization;
using Exception = System.Exception;

namespace TheCodePatch.PinMeTo.Client.Response;

internal class ExceptionFactory(ISerializer serializer, ILogger<ExceptionFactory> logger) : IExceptionFactory
{
    public Exception CreateException(HttpStatusCode statusCode, string responseContent)
    {
        var appropriateException = statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(responseContent),
            HttpStatusCode.BadRequest => BadRequest(responseContent),
            HttpStatusCode.BadGateway => BadGateway(),
            HttpStatusCode.UnprocessableEntity => UnprocessableEntity(responseContent),
            _ => null,
        };

        if (null != appropriateException)
        {
            return appropriateException;
        }

        return new PinMeToException("Unexpected exception from PinMeTo");
    }

    private Exception BadGateway()
    {
        logger.LogDebug("BadGateway returned from API");
        return new BadGatewayException();
    }
    
    private Exception NotFound(string responseContent)
    {
        logger.LogDebug("NotFound returned from API: {ResponseContent}", responseContent);
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
            var errorModel = serializer.Deserialize<ErrorResponse>(responseContent);
            if (errorModel.Error == "invalid_request")
            {
                logger.LogError(
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
            var validationErrors = serializer.Deserialize<
                AtomicResponse<Dictionary<string, List<string>>>
            >(responseContent);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (null != validationErrors.Data)
            {
                logger.LogWarning(
                    "Validation errors 1: {ValidationErrors}",
                    serializer.Serialize(validationErrors.Data)
                );
                exception = new ValidationErrorsException(validationErrors.Data);
                return true;
            }

            exception = null;
            return false;
        }

        bool TryDeserializeValidationErrors([NotNullWhen(true)] out Exception? e)
        {
            var validationErrors2 = serializer.Deserialize<Dictionary<string, List<string>>>(
                responseContent
            );
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (null != validationErrors2)
            {
                logger.LogWarning(
                    "Validation errors 2: {ValidationErrors}",
                    serializer.Serialize(validationErrors2)
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
