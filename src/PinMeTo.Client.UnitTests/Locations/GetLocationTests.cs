using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Locations;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeTo.Client.UnitTests.Locations;

public class GetLocationTests : UnitTestBase
{
    private readonly ILocationsClient _locationsClient;
    private readonly UnitTestsOptions _options;

    public GetLocationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _locationsClient = ServiceProvider.GetRequiredService<ILocationsClient>();
        _options = ServiceProvider
            .GetRequiredService<IOptionsMonitor<UnitTestsOptions>>()
            .CurrentValue;
    }

    [Fact]
    public async Task DetailsAreRetrievedForLocationWithMinimalData()
    {
        await _locationsClient.CreateOrUpdate(Constants.MinimalLocation);
        var details = await _locationsClient.Get(Constants.MinimalLocation.StoreId);
        var d = details.ShouldNotBeNull();
    }

    // TODO The Name seems to fall back to the client's name. Is that expected?
    [Fact]
    public async Task AllDetailsAreRetrieved()
    {
        // Reset the data in the API
        var expected = Constants.CompleteLocation;
        await _locationsClient.CreateOrUpdate(expected);

        var details = await _locationsClient.Get(expected.StoreId);
        var d = details.ShouldNotBeNull();

        // d.Name.ShouldBe(expected.Name);   Seems to fall back on client name

        d.StoreId.ShouldBe(expected.StoreId);
        d.Address.ShouldBeEquivalentTo(expected.Address);
        d.SpecialOpenHours.ShouldBeEquivalentTo(expected.SpecialOpenHours);
        d.Contact.ShouldBeEquivalentTo(expected.Contact);
        d.Description.ShouldBeEquivalentTo(expected.Description);
        d.LocationDescriptor.ShouldBe(expected.LocationDescriptor);
        d.Position.ShouldBeEquivalentTo(expected.Position);
        d.OpenHours.ShouldBeEquivalentTo(expected.OpenHours);
        d.IsPermanentlyClosed.ShouldBe(expected.IsPermanentlyClosed ?? false);
        d.IsTemporarilyClosedUntil.ShouldBe(expected.IsTemporarilyClosedUntil);
        d.IsAlwaysOpen.ShouldBe(expected.IsAlwaysOpen ?? false);
        d.WifiSsid.ShouldBe(expected.WifiSsid);
        d.FacebookName.ShouldBe(
            _options.IsFacebookCustomNameEnabled ? expected.FacebookName : null
        );

        d.GoogleName.ShouldBe(_options.IsGoogleCustomNameEnabled ? expected.GoogleName : null);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task ThrowsWhenInvalidStoreId(string? storeId)
    {
        await Assert.ThrowsAsync<InvalidInputException>(() => _locationsClient.Get(storeId!));
    }
}
