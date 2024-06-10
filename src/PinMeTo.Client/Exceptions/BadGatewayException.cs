namespace TheCodePatch.PinMeTo.Client.Exceptions;

/// <summary>
/// Thrown when PinMeTo returns bad gateway.
/// </summary>
public class BadGatewayException : Exception
{
    internal BadGatewayException() : base("Bad gateway") { }
}
