﻿namespace TheCodePatch.PinMeTo.Client.Exceptions;

/// <summary>
/// Thrown when the API indicates that an invalid request has been made.
/// </summary>
public class InvalidPinMeToRequestException : Exception
{
    internal InvalidPinMeToRequestException(string message) : base(message) { }
}
