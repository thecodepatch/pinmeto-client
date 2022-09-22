using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Locations;
using TheCodePatch.PinMeTo.Client.Locations.Model;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeTo.Client.UnitTests.Locations;

public class ListLocationsTests : UnitTestBase
{
    private readonly ILocationsClient _locationsClient;

    public ListLocationsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _locationsClient = ServiceProvider.GetRequiredService<ILocationsClient>();
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
            {
                prevNav.ShouldNotBeNull(
                    $"Page {pageNumber} did not have the expected previous navigation item"
                );
            }
            else
            {
                prevNav.ShouldBeNull(
                    $"Page {pageNumber} had an unexpected previous navigation item."
                );
            }

            nextNav = result.NextPage;
            if (expectNext.HasValue)
            {
                if (expectNext.Value)
                {
                    nextNav.ShouldNotBeNull(
                        $"Page {pageNumber} did not have the expected next navigation item"
                    );
                }
                else
                {
                    nextNav.ShouldBeNull(
                        $"Page {pageNumber} had an unexpected next navigation item."
                    );
                }
            }
        }
    }

    [Fact]
    public async Task ThrowsWhenPageSizeIsOutOfBounds()
    {
        await Assert.ThrowsAsync<InvalidInputException>(() => _locationsClient.List(new(251)));
    }

    [Fact]
    public async Task AllPropertiesAreSetAsExpected()
    {
        List<Location> allLocations = await GetAllLocations();

        new PropertyTester<Location>(allLocations)
            .NoneShouldBeNullOrWhiteSpace(x => x.StoreId)
            .NoneShouldBeNullOrWhiteSpace(x => x.Name)
            .SomeShouldNotBeNullOrWhiteSpace(x => x.LocationDescriptor)
            .NoneShouldBeNull(
                x => x.Address,
                address =>
                    address
                        .NoneShouldBeNullOrWhiteSpace(a => a.Street)
                        .NoneShouldBeNullOrWhiteSpace(a => a.Zip)
                        .NoneShouldBeNullOrWhiteSpace(a => a.City)
                        .SomeShouldNotBeNullOrWhiteSpace(a => a.State)
                        .NoneShouldBeNullOrWhiteSpace(a => a.Country)
            )
            .NoneShouldBeNull(
                x => x.SpecialOpeningHours,
                allowEmptyEnumerables: true,
                specialOpeningHours =>
                    specialOpeningHours
                        .NoneShouldEqual(soh => soh.CloseTime, default)
                        .NoneShouldEqual(soh => soh.OpenTime, default)
                        .NoneShouldEqual(soh => soh.Start, default)
                        .NoneShouldEqual(soh => soh.End, default)
                        .NoneShouldBeNullOrWhiteSpace(soh => soh.Label)
            )
            .NoneShouldBeNull(
                x => x.Contact,
                contact =>
                    contact
                        .SomeShouldNotBeNullOrWhiteSpace(c => c.Email)
                        .SomeShouldNotBeNullOrWhiteSpace(c => c.Homepage)
                        .SomeShouldNotBeNullOrWhiteSpace(c => c.Phone)
            )
            .NoneShouldBeNull(
                x => x.Description,
                description =>
                    description
                        .SomeShouldNotBeNullOrWhiteSpace(d => d!.Short)
                        .SomeShouldNotBeNullOrWhiteSpace(d => d!.Long)
            )
            .NoneShouldBeNull(
                x => x.OpeningHours,
                openHours =>
                    openHours
                        .NoneShouldBeNull(o => o!.Monday, TestDay)
                        .NoneShouldBeNull(o => o!.Tuesday, TestDay)
                        .NoneShouldBeNull(o => o!.Wednesday, TestDay)
                        .NoneShouldBeNull(o => o!.Thursday, TestDay)
                        .NoneShouldBeNull(o => o!.Friday, TestDay)
                        .NoneShouldBeNull(o => o!.Saturday, TestDay)
                        .NoneShouldBeNull(o => o!.Sunday, TestDay)
            )
            .NoneShouldBeNull(
                x => x.Position,
                position =>
                    position
                        .NoneShouldEqual(p => p.Latitude, 0)
                        .NoneShouldEqual(p => p.Longitude, 0)
            )
            .SomeShouldNotBeNullOrWhiteSpace(x => x.OpeningDate)
            .NoneShouldBeNull(
                x => x.Network,
                network =>
                    network
                        .SomeShouldNotBeNull(
                            n => n.Facebook,
                            facebook =>
                                facebook
                                    .NoneShouldBeNullOrWhiteSpace(f => f.CoverImage)
                                    .NoneShouldBeNullOrWhiteSpace(f => f.Link)
                                    .NoneShouldBeNullOrWhiteSpace(f => f.PageId)
                                    .NoneShouldBeNullOrWhiteSpace(f => f.ProfileImage)
                        )
                        .SomeShouldNotBeNull(
                            n => n.Google,
                            google =>
                                google
                                    .NoneShouldBeNullOrWhiteSpace(g => g.CoverImage)
                                    .NoneShouldBeNullOrWhiteSpace(g => g.Link)
                                    .NoneShouldBeNullOrWhiteSpace(g => g.NewReviewUrl)
                                    .NoneShouldBeNullOrWhiteSpace(g => g.PlaceId)
                                    .NoneShouldBeNullOrWhiteSpace(g => g.ProfileImage)
                        )
                        .SomeShouldNotBeNullOrWhiteSpace(x => x.WifiSsid)
            )
            .ShouldHaveNoErrors();

        void TestDay(PropertyTester<WeekOpeningHours.DayOpeningHours> dayToTest)
        {
            dayToTest
            // We are not testing State because there are no invalid values.
            .NoneShouldBeNull(
                m => m.Times,
                allowEmptyEnumerables: true,
                times =>
                    times
                        .NoneShouldEqual(t => t.Opens, default)
                        .NoneShouldBeNull(t => t.Closes, default)
            );
        }
    }

    private async Task<List<Location>> GetAllLocations()
    {
        var retval = new List<Location>();
        var nextPage = new PageNavigation(250);
        do
        {
            var result = await _locationsClient.List(nextPage);
            retval.AddRange(result.Items);
            nextPage = result.NextPage;
        } while (null != nextPage);

        return retval;
    }
}
