﻿using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TheCodePatch.PinMeToClient.Locations;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeToClient.UnitTests.Locations;

public class CreateLocationTests : UnitTestBase
{
    private readonly ILocationsClient _locationsClient;

    public CreateLocationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _locationsClient = ServiceProvider.GetRequiredService<ILocationsClient>();
    }

    [Fact]
    public async Task CanCreateCompleteLocation()
    {
        await _locationsClient.CreateOrUpdate(Constants.CompleteLocation);
    }
}
