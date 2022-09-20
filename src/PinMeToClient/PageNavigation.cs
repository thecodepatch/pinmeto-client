using System.Collections.Generic;

namespace TheCodePatch.PinMeToClient;

/// <summary>
/// Indicates how to navigate between pages.
/// </summary>
/// <param name="PageSize">The page size.</param>
/// <param name="Direction">The direction in which to navigate, if we are navigating.</param>
/// <param name="Key">The key of the page to navigate to, if we are navigating..</param>
public record PageNavigation(
    uint PageSize = 100,
    PageNavigationDirection? Direction = null,
    string? Key = null
);
