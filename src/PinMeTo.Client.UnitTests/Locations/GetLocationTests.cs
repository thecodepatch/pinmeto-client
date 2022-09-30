using System.Collections.Generic;
using DiffEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Locations;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeTo.Client.UnitTests.Locations;

public class GetLocationTests : UnitTestBase
{
    private readonly ILocationsService<TestCustomData> _locationsService;
    private readonly UnitTestsOptions _options;

    public GetLocationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _locationsService = ServiceProvider.GetRequiredService<ILocationsService<TestCustomData>>();
        _options = ServiceProvider
            .GetRequiredService<IOptionsMonitor<UnitTestsOptions>>()
            .CurrentValue;
    }

    [Fact]
    public async Task GettingNonExistentLocationThrowsException()
    {
        await Assert.ThrowsAsync<NotFoundException>(
            () => _locationsService.Get("NonExistentStoreId")
        );
    }

    [Fact]
    public async Task DetailsAreRetrievedForLocationWithMinimalData()
    {
        await _locationsService.CreateOrUpdate(Constants.MinimalLocation);
        var details = await _locationsService.Get(Constants.MinimalLocation.StoreId);
        details.ShouldNotBeNull();
    }

    // TODO The Name seems to fall back to the client's name. Is that expected?
    [Fact]
    public async Task AllDetailsAreRetrieved()
    {
        // Reset the data in the API
        var expected = Constants.CompleteLocation;
        await _locationsService.CreateOrUpdate(expected);

        var details = await _locationsService.Get(expected.StoreId);
        var d = details.ShouldNotBeNull();

        // d.Name.ShouldBe(expected.Name);   Seems to fall back on client name

        d.Result.StoreId.ShouldBe(expected.StoreId);

        d.Result.Address.City.ShouldNotBeNullOrWhiteSpace();
        d.Result.Address.Country.ShouldNotBeNullOrWhiteSpace();
        d.Result.Address.State.ShouldNotBeNullOrWhiteSpace();
        d.Result.Address.Street.ShouldNotBeNullOrWhiteSpace();
        d.Result.Address.Zip.ShouldNotBeNullOrWhiteSpace();

        d.Result.Position.ShouldNotBeNull().Latitude.ShouldNotBe(default);
        d.Result.Position.ShouldNotBeNull().Longitude.ShouldNotBe(default);

        d.Result.SpecialOpeningHours.ShouldBeEquivalentTo(expected.SpecialOpeningHours);
        d.Result.Contact.ShouldBeEquivalentTo(expected.Contact);
        d.Result.Description.ShouldBeEquivalentTo(expected.Description);
        d.Result.LocationDescriptor.ShouldBe(expected.LocationDescriptor);
        d.Result.OpeningHours.ShouldBeEquivalentTo(expected.OpeningHours);
        d.Result.IsPermanentlyClosed.ShouldBe(expected.IsPermanentlyClosed ?? false);
        d.Result.IsTemporarilyClosedUntil.ShouldBe(expected.IsTemporarilyClosedUntil);
        d.Result.IsAlwaysOpen.ShouldBe(expected.IsAlwaysOpen ?? false);
        d.Result.WifiSsid.ShouldBe(expected.WifiSsid);
        d.Result.FacebookName.ShouldBe(
            _options.IsFacebookCustomNameEnabled ? expected.FacebookName : null
        );
        d.Result.CustomData.ShouldBeEquivalentTo(expected.CustomData);
        d.Result.GoogleName.ShouldBe(
            _options.IsGoogleCustomNameEnabled ? expected.GoogleName : null
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task ThrowsWhenInvalidStoreId(string? storeId)
    {
        await Assert.ThrowsAsync<InvalidInputException>(() => _locationsService.Get(storeId!));
    }
}
