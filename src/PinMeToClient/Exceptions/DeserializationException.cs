namespace TheCodePatch.PinMeToClient.Exceptions;

public class DeserializationException : Exception
{
    public DeserializationException() : base("Failed to deserialize") { }
}
