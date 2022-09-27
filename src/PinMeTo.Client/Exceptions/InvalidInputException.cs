using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.Exceptions;

/// <summary>
///     An exception indicating that the user has provided invalid input in a request.
/// </summary>
public class InvalidInputException : Exception
{
    internal InvalidInputException(string parameterName, string description)
        : base($"Validation failed for parameter {parameterName}: {description}") { }
}
