using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.Exceptions;

/// <summary>
/// Thrown when a required property was missing.
/// </summary>
public class MissingRequiredPropertyException : Exception
{
    internal MissingRequiredPropertyException(string message):base(message)
    {
        
    }
}
