using System.Collections.Generic;

namespace TheCodePatch.PinMeTo.Client.Exceptions;

/// <summary>
/// Thrown when a deserialization fails.
/// </summary>
public class DeserializationException : Exception
{
    internal DeserializationException() : base("Failed to deserialize") { }
}
