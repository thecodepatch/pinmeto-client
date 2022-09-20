using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using TheCodePatch.PinMeToClient.Exceptions;
using TheCodePatch.PinMeToClient.Locations;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeToClient.UnitTests.Locations;

public class LocationsTests : UnitTestBase
{
    private readonly ILocationsClient _locationsClient;
    private readonly UnitTestsOptions _options;

    public LocationsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _locationsClient = ServiceProvider.GetRequiredService<ILocationsClient>();
        _options = ServiceProvider
            .GetRequiredService<IOptionsMonitor<UnitTestsOptions>>()
            .CurrentValue;
    }

    [Fact]
    public async Task CanList()
    {
        var response = await _locationsClient.List(new PageNavigation(100));
        response.Items.ShouldNotBeEmpty();
    }

    // TODO, This fails as we start navigating backwards. Is there anything wrong in the API?
    [Fact]
    public async Task CanNavigatePages()
    {
        var allSamplePagesResponse = await _locationsClient.List(new(3));
        allSamplePagesResponse.Items.Count.ShouldBe(3, "This test requires at least 3 locations.");

        var page1Result = await _locationsClient.List(new(1));
        AssertResult(page1Result, 1, false, true, out _, out var nextIsPage2Nav);

        var page2Result = await _locationsClient.List(nextIsPage2Nav!);
        AssertResult(page2Result, 2, true, true, out _, out var nextIsPage3Nav);

        var page3Result = await _locationsClient.List(nextIsPage3Nav!);
        AssertResult(page3Result, 3, true, null, out var prevIsPage2Nav, out _);

        // Turn around and reverse down towards page 1.
        page2Result = await _locationsClient.List(prevIsPage2Nav!);
        AssertResult(page2Result, 2, true, true, out var prevIsPage1Nav, out nextIsPage3Nav);

        page1Result = await _locationsClient.List(prevIsPage1Nav!);
        AssertResult(page1Result, 1, false, true, out _, out _);

        void AssertResult(
            PagedResult<Location> result,
            int pageNumber,
            bool expectPrev,
            bool? expectNext,
            out PageNavigation? prevNav,
            out PageNavigation? nextNav
        )
        {
            var theItem = result.Items.ShouldHaveSingleItem(
                $"Page {pageNumber} did not have a single number"
            );
            var matchingPageFromAllPagesResponse = allSamplePagesResponse.Items[pageNumber - 1];
            theItem.StoreId.ShouldBe(
                matchingPageFromAllPagesResponse.StoreId,
                $"Page {pageNumber} is not the same item as the corresponding position in the all sample pages response."
            );

            prevNav = result.PreviousPage;
            if (expectPrev)
                prevNav.ShouldNotBeNull(
                    $"Page {pageNumber} did not have the expected previous navigation item"
                );
            else
                prevNav.ShouldBeNull(
                    $"Page {pageNumber} had an unexpected previous navigation item."
                );

            nextNav = result.NextPage;
            if (expectNext.HasValue)
            {
                if (expectNext.Value)
                    nextNav.ShouldNotBeNull(
                        $"Page {pageNumber} did not have the expected next navigation item"
                    );
                else
                    nextNav.ShouldBeNull(
                        $"Page {pageNumber} had an unexpected next navigation item."
                    );
            }
        }
    }

    [Fact]
    public async Task ThrowsWhenPageSizeIsOutOfBounds()
    {
        await Assert.ThrowsAsync<InvalidInputException>(() => _locationsClient.List(new(251)));
    }

    [Fact]
    public async Task CanGetDetails()
    {
        var details = await _locationsClient.Get(_options.AnExistingStoreId);
        details.ShouldNotBeNull().StoreId.ShouldNotBeNullOrWhiteSpace();
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
