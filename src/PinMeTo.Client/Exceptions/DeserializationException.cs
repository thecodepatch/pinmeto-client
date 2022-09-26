using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.Exceptions;

/// <summary>
/// Thrown when a deserialization fails.
/// </summary>
public class DeserializationException : Exception
{
    public DeserializationException() : base("Failed to deserialize") { }
}
