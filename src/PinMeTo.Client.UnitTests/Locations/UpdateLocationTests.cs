﻿using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using TheCodePatch.PinMeTo.Client.Locations;
using TheCodePatch.PinMeTo.Client.Locations.Model;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeTo.Client.UnitTests.Locations;

public class UpdateLocationTests : UnitTestBase
{
    private readonly ILocationsClient _locationsClient;
    private readonly IOptionsMonitor<UnitTestsOptions> _options;

    public UpdateLocationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _locationsClient = ServiceProvider.GetRequiredService<ILocationsClient>();
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
                        State = WeekOpeningHours.DayOpeningHours.OpenState.Open,
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
                new List<SpecialOpeningHour>
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
        Func<LocationDetails, TValueToUpdate> detailsValuePropertySelector,
        Func<TValueToUpdate, TValueToUpdate> valueModifier,
        Expression<Func<UpdateLocationInput, TValueToUpdate>> updateInputPropertySelector,
        Func<PendingChanges, TValueToUpdate>? pendingChangesValueSelector = null
    )
    {
        // Reset the data in the API and get the baseline data.
        var initial = await _locationsClient.CreateOrUpdate(Constants.CompleteLocation);

        var valueFromBaseline = detailsValuePropertySelector(initial);
        var modifiedValue = valueModifier(valueFromBaseline);

        // Set the modified value to the update input data.
        var updateInput = new UpdateLocationInput();
        SetPropertyValue(updateInput, updateInputPropertySelector, modifiedValue);

        // Perform the update.
        var updateResult = await _locationsClient.UpdateLocation(
            Constants.CompleteLocation.StoreId,
            updateInput
        );

        var detailsResult = await _locationsClient.Get(Constants.CompleteLocation.StoreId);

        if (null == pendingChangesValueSelector)
        {
            // Check that the return value from UpdateLocation has had the value updated.
            detailsValuePropertySelector(updateResult).ShouldBeEquivalentTo(modifiedValue);
            detailsValuePropertySelector(detailsResult).ShouldBeEquivalentTo(modifiedValue);
        }
        else
        {
            pendingChangesValueSelector(updateResult.PendingChanges)
                .ShouldBeEquivalentTo(modifiedValue);
            pendingChangesValueSelector(detailsResult.PendingChanges)
                .ShouldBeEquivalentTo(modifiedValue);
        }
    }

    private static void SetPropertyValue<T, TValue>(
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
