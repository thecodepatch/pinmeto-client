﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Response;

/// <summary>
///     Model for the error messages returned by the API.
/// </summary>
internal record ErrorResponse
{
    [JsonPropertyName("error_description")]
    public string Description { get; init; } = null!;

    [JsonPropertyName("code")]
    public int Code { get; init; }

    [JsonPropertyName("error")]
    public string Error { get; init; } = null!;
}
