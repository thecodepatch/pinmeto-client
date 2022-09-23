using System.Collections.Generic;
using TheCodePatch.PinMeTo.Client.Locations.Model;

namespace TheCodePatch.PinMeTo.Client.UnitTests.Locations;

internal static class Constants
{
    public static readonly CreateLocationInput MinimalLocation =
        new()
        {
            StoreId = "UNIT-TEST-STORE-NOT-REAL-MINIMAL",
            Address =
            {
                City = "Some city",
                Country = "Sweden",
                Street = "Some street",
                Zip = "Some zip",
            },
        };

    public static readonly CreateLocationInput CompleteLocation =
        new()
        {
            StoreId = "UNIT-TEST-STORE-NOT-REAL",
            Address =
            {
                City = "Some city",
                Country = "Sweden",
                State = "Some state",
                Street = "Some street",
                Zip = "Some zip",
            },
            Contact =
            {
                Email = "someemail@email.com",
                Homepage = "http://www.somehomepage.com",
                Phone = "123123123",
            },
            Description = new() { Long = "a long description", Short = "a short description" },
            Name = "A test location",
            Position = { Latitude = 56.2328943, Longitude = 12.8168534 },
            CustomData = null,
            FacebookName = "fbname",
            GoogleName = "some google name",
            LocationDescriptor = "some location descriptor",
            OpeningHours = new()
            {
                Monday = CreateDay(1),
                Tuesday = CreateDay(2),
                Wednesday = CreateDay(3),
                Thursday = CreateDay(4),
                Friday = CreateDay(5),
                Saturday = CreateDay(6),
                Sunday = CreateDay(7),
            },
            WifiSsid = "some wifi ssid",
            IsAlwaysOpen = false,
            IsPermanentlyClosed = false,
            SpecialOpeningHours = new()
            {
                new()
                {
                    Label = "christmas eve",
                    Start = new DateOnly(DateTime.Now.Year, 12, 24),
                    End = new DateOnly(DateTime.Now.Year, 12, 24),
                    OpenTime = new TimeOnly(7, 0),
                    CloseTime = new TimeOnly(23, 0),
                    IsClosed = false,
                },
            },
            IsTemporarilyClosedUntil = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
        };

    private static DayOpeningHours CreateDay(int openingTime)
    {
        return new()
        {
            State = OpenState.Open,
            Times =
            {
                new()
                {
                    Opens = new TimeOnly(openingTime, 0),
                    Closes = new TimeOnly(openingTime + 1, 0),
                },
            },
        };
    }
}
