﻿using System.Text.Json;

namespace TheCodePatch.PinMeTo.Client.Exceptions;

/// <summary>
/// Thrown when there are validation errors.
/// </summary>
public class ValidationErrorsException : Exception
{
    /// <summary>
    /// The validation errors.
    /// </summary>
    public IDictionary<string, List<string>> ValidationErrors { get; }

    internal ValidationErrorsException(IDictionary<string, List<string>> validationErrors)
        : base("Validation error(s) occurred: " + JsonSerializer.Serialize(validationErrors))
    {
        ValidationErrors = validationErrors;
    }
}
