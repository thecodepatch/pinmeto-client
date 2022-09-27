using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Locations;
using TheCodePatch.PinMeTo.Client.Locations.Model;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeTo.Client.UnitTests.Locations;

public class UpdateLocationTests : UnitTestBase
{
    private readonly ILocationsService<TestCustomData> _locationsService;
    private readonly IOptionsMonitor<UnitTestsOptions> _options;

    public UpdateLocationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _locationsService = ServiceProvider.GetRequiredService<ILocationsService<TestCustomData>>();
        _options = ServiceProvider.GetRequiredService<IOptionsMonitor<UnitTestsOptions>>();
    }

    [Fact]
    public async Task CanUpdateLocationDescriptor()
    {
        await TestSinglePropertyUpdate(
            details => details.LocationDescriptor,
            oldVal => oldVal + "_MOD",
            input => input.LocationDescriptor
        );
    }

    [Fact]
    public async Task CanUpdateDescription()
    {
        await TestSinglePropertyUpdate(
            details => details.Description,
            d => new() { Long = d?.Long + "_MOD", Short = d?.Short + "_MOD" },
            input => input.Description
        );
    }

    [Fact]
    public async Task CanUpdateContact()
    {
        await TestSinglePropertyUpdate(
            details => details.Contact,
            d =>
                new Contact
                {
                    Email = string.IsNullOrWhiteSpace(d?.Email)
                      ? "someemail@example.com"
                      : d.Email + "mod", //Must be a valid email address.
                    Homepage = d?.Homepage + "_MOD",
                    Phone = d?.Phone + "000", // Must be a valid phone number
                },
            input => input.Contact
        );
    }

    [Fact]
    public async Task CanUpdateAddress()
    {
        await TestSinglePropertyUpdate(
            details => details.Address,
            a =>
                new Address
                {
                    City = a?.City + "_MOD",
                    Country = "United Kingdom", // Must be a valid country
                    State = a?.State + "_MOD",
                    Street = a?.Street + "_MOD",
                    Zip = a?.Zip + "_MOD",
                },
            input => input.Address,
            pendingChanges => pendingChanges.Address
        );
    }

    [Fact]
    public async Task CanUpdateIsPermanentlyClosed()
    {
        await TestSinglePropertyUpdate(
            details => details.IsPermanentlyClosed,
            oldVal => !oldVal,
            input => input.IsPermanentlyClosed
        );
    }

    [Fact]
    public async Task CanUpdateIsTemporarilyClosedUntil()
    {
        await TestSinglePropertyUpdate(
            details => details.IsTemporarilyClosedUntil,
            _ => DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            input => input.IsTemporarilyClosedUntil
        );
    }

    [Fact]
    public async Task CanUpdateIsAlwaysOpen()
    {
        await TestSinglePropertyUpdate(
            details => details.IsAlwaysOpen,
            oldValue => !oldValue,
            input => input.IsAlwaysOpen
        );
    }

    [Fact]
    public async Task CanUpdatePosition()
    {
        await TestSinglePropertyUpdate(
            details => details.Position,
            _ => new() { Latitude = 69.0600002, Longitude = 20.5490133 },
            input => input.Position,
            pendingChanges => pendingChanges.Position
        );
    }

    [Fact]
    public async Task CanUpdateOpeningHours()
    {
        await TestSinglePropertyUpdate(
            details => details.OpeningHours,
            oldValue =>
                oldValue! with
                {
                    Monday = new()
                    {
                        State = OpenState.Open,
                        Times =
                        {
                            new() { Opens = new TimeOnly(12, 0), Closes = new TimeOnly(23, 0) },
                        },
                    },
                },
            input => input.OpeningHours
        );
    }

    [Fact]
    public async Task CanUpdateSpecialOpeningHours()
    {
        await TestSinglePropertyUpdate(
            details => details.SpecialOpeningHours,
            _ =>
                new List<SpecialOpeningHours>
                {
                    new()
                    {
                        Start = DateOnly.FromDateTime(DateTime.Today.AddDays(100)),
                        End = DateOnly.FromDateTime(DateTime.Today.AddDays(100)),
                        Label = "Something new",
                        CloseTime = new TimeOnly(20, 0),
                        IsClosed = false,
                        OpenTime = new TimeOnly(10, 0),
                    },
                },
            input => input.SpecialOpeningHours
        );
    }

    [Fact]
    public async Task CanUpdateGoogleName()
    {
        if (_options.CurrentValue.IsGoogleCustomNameEnabled)
        {
            await TestSinglePropertyUpdate(
                details => details.GoogleName,
                oldVal => oldVal + "_MOD",
                input => input.GoogleName
            );
        }
    }

    [Fact]
    public async Task CanUpdateWifiSsid()
    {
        await TestSinglePropertyUpdate(
            details => details.WifiSsid,
            oldVal => oldVal + "_MOD",
            input => input.WifiSsid
        );
    }

    [Fact]
    public async Task CanUpdateFacebookName()
    {
        if (_options.CurrentValue.IsFacebookCustomNameEnabled)
        {
            await TestSinglePropertyUpdate(
                details => details.LocationDescriptor,
                oldVal => oldVal + "_MOD",
                input => input.LocationDescriptor
            );
        }
    }

    async Task TestSinglePropertyUpdate<TValueToUpdate>(
        Func<LocationDetails<TestCustomData>, TValueToUpdate> detailsValuePropertySelector,
        Func<TValueToUpdate, TValueToUpdate> valueModifier,
        Expression<
            Func<UpdateLocationInput<TestCustomData>, TValueToUpdate>
        > updateInputPropertySelector,
        Func<PendingChanges, TValueToUpdate>? pendingChangesValueSelector = null
    )
    {
        // Reset the data in the API and get the baseline data.
        var initial = await _locationsService.CreateOrUpdate(Constants.CompleteLocation);

        var valueFromBaseline = detailsValuePropertySelector(initial.Result);
        var modifiedValue = valueModifier(valueFromBaseline);

        // Set the modified value to the update input data.
        var updateInput = new UpdateLocationInput<TestCustomData>();
        SetPropertyValue(updateInput, updateInputPropertySelector, modifiedValue);

        // Perform the update.
        var updateResult = await _locationsService.UpdateLocation(
            Constants.CompleteLocation.StoreId,
            updateInput
        );

        var detailsResult = await _locationsService.Get(Constants.CompleteLocation.StoreId);

        if (null == pendingChangesValueSelector)
        {
            // Check that the return value from UpdateLocation has had the value updated.
            detailsValuePropertySelector(updateResult.Result).ShouldBeEquivalentTo(modifiedValue);
            detailsValuePropertySelector(detailsResult.Result).ShouldBeEquivalentTo(modifiedValue);
        }
        else
        {
            pendingChangesValueSelector(updateResult.Result.PendingChanges)
                .ShouldBeEquivalentTo(modifiedValue);
            pendingChangesValueSelector(detailsResult.Result.PendingChanges)
                .ShouldBeEquivalentTo(modifiedValue);
        }

        static void SetPropertyValue<T, TValue>(
            T target,
            Expression<Func<T, TValue>> memberLamda,
            TValue value
        )
        {
            if (memberLamda.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value, null);
                }
            }
        }
    }

    [Fact]
    public async Task CanAddOpeningTimeAfterSettingClosed()
    {
        var initial = Constants.MinimalLocation with
        {
            StoreId = "UNIT-TEST-CanAddOpeningTimeAfterSettingClosed",
        };
        var details = await _locationsService.CreateOrUpdate(initial);

        details.Result.OpeningHours.Monday.State.ShouldBe(OpenState.NotSpecified);
        details.Result.OpeningHours.Monday.Times.ShouldBeEmpty();

        var openingHours = new OpeningHours
        {
            Opens = new TimeOnly(10, 0),
            Closes = new TimeOnly(17, 0),
        };

        // Set state to open and assign a time
        details = await _locationsService.UpdateLocation(
            details.Result.StoreId,
            new()
            {
                OpeningHours = new()
                {
                    Monday = { State = OpenState.Open, Times = { openingHours } },
                },
            }
        );
        details.Result.OpeningHours.Monday.State.ShouldBe(OpenState.Open);
        details.Result.OpeningHours.Monday.Times
            .ShouldHaveSingleItem()
            .ShouldBeEquivalentTo(openingHours);

        // Now set to closed
        details = await _locationsService.UpdateLocation(
            details.Result.StoreId,
            new() { OpeningHours = new() { Monday = { State = OpenState.Closed } } }
        );
        details.Result.OpeningHours.Monday.State.ShouldBe(OpenState.Closed);
        details.Result.OpeningHours.Monday.Times.ShouldBeEmpty();

        // Now set to open again
        details = await _locationsService.UpdateLocation(
            details.Result.StoreId,
            new()
            {
                OpeningHours = new()
                {
                    Monday = { State = OpenState.Open, Times = { openingHours } },
                },
            }
        );

        details.Result.OpeningHours.ShouldNotBeNull().Monday.State.ShouldBe(OpenState.Open);
        details.Result.OpeningHours.Monday.Times
            .ShouldHaveSingleItem()
            .ShouldBeEquivalentTo(openingHours);
    }

    [Fact]
    public async Task NonExistentLocationThrowsException()
    {
        await Assert.ThrowsAsync<NotFoundException>(
            () =>
                _locationsService.UpdateLocation(
                    "NONEXISTENT-LOCATION",
                    new() { Description = new() { Short = "test" } }
                )
        );
    }
}
