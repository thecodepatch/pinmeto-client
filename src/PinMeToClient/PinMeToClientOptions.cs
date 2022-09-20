using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheCodePatch.PinMeToClient;

/// <summary>
///     Options for the PinMeTo client.
/// </summary>
public record PinMeToClientOptions
{
    /// <summary>
    ///     The base URI to the PinMeTo API.
    ///     Typically this is https://api.test.pinmeto.com for the test environment and
    ///     https://api.pinmeto.com for the production environment. Read more in PinMeTo's
    ///     own documentation on https://github.com/PinMeTo/documentation/wiki/Api-Documentation.
    /// </summary>
    [Required]
    public Uri ApiBaseAddress { get; set; } = null!;

    /// <summary>
    ///     The AppID provided by PinMeTo.
    /// </summary>
    [Required]
    public string AppId { get; set; } = null!;

    /// <summary>
    ///     The App secret provided by PinMeTo.
    /// </summary>
    [Required]
    public string AppSecret { get; set; } = null!;
}
