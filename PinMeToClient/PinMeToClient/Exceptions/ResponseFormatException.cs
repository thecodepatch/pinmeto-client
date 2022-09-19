using System.Collections.Generic;

namespace TheCodePatch.PinMeToClient.Exceptions;

public class ResponseFormatException : Exception
{
    internal ResponseFormatException(string problemDescription)
        : base("The response had an invalid format. " + problemDescription) { }
}