using System.Collections.Generic;

namespace TheCodePatch.PinMeToClient.Exceptions;

/// <summary>
/// An exception indicating that the response has an unrecognized format.
/// </summary>
public class ResponseFormatException : Exception
{
    internal ResponseFormatException(string problemDescription)
        : base("The response had an invalid format. " + problemDescription) { }
}
