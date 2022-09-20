using System.ComponentModel.DataAnnotations;

namespace TheCodePatch.PinMeToClient.UnitTests;

public record UnitTestsOptions
{
    [Required]
    public string AnExistingStoreId { get; init; } = null!;
}
