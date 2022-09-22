using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheCodePatch.PinMeTo.Client.UnitTests;

public record UnitTestsOptions
{
    [Required]
    public bool IsFacebookCustomNameEnabled { get; init; }

    [Required]
    public bool IsGoogleCustomNameEnabled { get; init; }
}
