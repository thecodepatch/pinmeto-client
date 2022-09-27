namespace TheCodePatch.PinMeTo.Client.Exceptions;

/// <summary>
/// Thrown when a requested resource could not be found.
/// </summary>
public class NotFoundException : Exception
{
    internal NotFoundException(string message) : base(message) { }
}
