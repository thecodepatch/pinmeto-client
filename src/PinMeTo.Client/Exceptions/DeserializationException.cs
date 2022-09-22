using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.Exceptions;

public class DeserializationException : Exception
{
    public DeserializationException() : base("Failed to deserialize") { }
}
