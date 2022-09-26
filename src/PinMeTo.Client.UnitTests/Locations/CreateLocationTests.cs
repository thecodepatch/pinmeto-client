using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TheCodePatch.PinMeTo.Client.Locations;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeTo.Client.UnitTests.Locations;

public class CreateLocationTests : UnitTestBase
{
    private readonly ILocationsService _locationsService;

    public CreateLocationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _locationsService = ServiceProvider.GetRequiredService<ILocationsService>();
    }

    [Fact]
    public async Task CanCreateCompleteLocation()
    {
        await _locationsService.CreateOrUpdate(Constants.CompleteLocation);
    }

    [Fact]
    public async Task CanCreateMinimalLocation()
    {
        await _locationsService.CreateOrUpdate(Constants.MinimalLocation);
    }
}
