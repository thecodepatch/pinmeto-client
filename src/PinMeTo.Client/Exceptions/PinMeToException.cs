using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.Exceptions;

/// <summary>
/// A general exception.
/// </summary>
public class PinMeToException : Exception
{
    public PinMeToException(string message) : base(message) { }
}
