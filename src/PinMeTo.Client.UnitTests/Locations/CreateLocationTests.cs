using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Locations;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeTo.Client.UnitTests.Locations;

public class CreateLocationTests : UnitTestBase
{
    private readonly ILocationsService<TestCustomData> _locationsService;

    public CreateLocationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _locationsService = ServiceProvider.GetRequiredService<ILocationsService<TestCustomData>>();
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

    [Fact]
    public async Task CreatingExistingStoreThrowsException()
    {
        await _locationsService.CreateOrUpdate(Constants.MinimalLocation);
        var ex = await Assert.ThrowsAsync<ValidationErrorsException>(
            () => _locationsService.Create(Constants.MinimalLocation)
        );

        var error = ex.ValidationErrors.ShouldHaveSingleItem();
        error.Key.ShouldBe("storeId");
        error.Value.ShouldHaveSingleItem().ShouldBe("StoreId must be unique");
    }

    [Fact]
    public async Task MissingRequiredPropertyThrowsException()
    {
        var withMissingProp = Constants.MinimalLocation with { Address = null! };
        var ex = await Assert.ThrowsAsync<MissingRequiredPropertyException>(
            () => _locationsService.Create(withMissingProp)
        );
        ex.Message.ShouldContain("address");
    }
}
