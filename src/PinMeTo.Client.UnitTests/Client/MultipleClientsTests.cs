using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Locations;
using TheCodePatch.PinMeTo.Client.UnitTests.Locations;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeTo.Client.UnitTests.Client;

public class MultipleClientsTests : UnitTestBase
{
    private readonly ILocationsService<TestCustomData> _defaultLocationService;
    private readonly ILocationsService<InvalidInstanceTestCustomData> _lastRegisteredService;

    public MultipleClientsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _defaultLocationService = GetService<TestCustomData>();
        _lastRegisteredService = GetService<InvalidInstanceTestCustomData>();
    }

    private ILocationsService<TCustomData> GetService<TCustomData>()
    {
        return ServiceProvider.GetRequiredService<ILocationsService<TCustomData>>();
    }

    [Fact]
    public async Task LastRegisteredClientDoesNotTakePrecedence()
    {
        var result = await _defaultLocationService.List(new PageNavigation(5));
        result.Result.Items.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task LastRegisteredClientCanBeUsed()
    {
        // The last registered client does not have valid connection data, and using it will
        // throw an exception. By verifying the exception, we assert that the last registered
        // service was indeed used.
        await Assert.ThrowsAsync<PinMeToException>(
            () => _lastRegisteredService.List(new PageNavigation(5))
        );
    }
}
