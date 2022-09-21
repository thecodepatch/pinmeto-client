using System.ComponentModel.DataAnnotations;

namespace TheCodePatch.PinMeToClient.UnitTests;

public record UnitTestsOptions
{
    [Required]
    public bool IsFacebookCustomNameEnabled { get; init; }

    [Required]
    public bool IsGoogleCustomNameEnabled { get; init; }
}
