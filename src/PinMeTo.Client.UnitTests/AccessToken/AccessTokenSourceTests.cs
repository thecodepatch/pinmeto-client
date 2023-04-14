using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheCodePatch.PinMeTo.Client.AccessToken;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.UnitTests.Locations;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeTo.Client.UnitTests.AccessToken;

public class AccessTokenSourceTests : UnitTestBase
{
    private readonly IAccessTokenSource<TestCustomData> _accessTokenSource;
    private readonly IAccessTokenSource<InvalidInstanceTestCustomData> _invalidAccessTokenSource;

    public AccessTokenSourceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _accessTokenSource = ServiceProvider.GetRequiredService<
            IAccessTokenSource<TestCustomData>
        >();
        _invalidAccessTokenSource = ServiceProvider.GetRequiredService<
            IAccessTokenSource<InvalidInstanceTestCustomData>
        >();
    }

    [Fact]
    public async Task ConsecutiveTokenRetrievalsAreFast()
    {
        await _accessTokenSource.GetAccessToken();

        var s = Stopwatch.StartNew();
        for (var i = 0; i < 10; i++)
        {
            await _accessTokenSource.GetAccessToken();
        }

        s.ElapsedMilliseconds.ShouldBeLessThan(5);
    }

    [Fact]
    public async Task TokenIsRetrievedWithExpectedValues()
    {
        var t = await _accessTokenSource.GetAccessToken();
        t.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task TokensAreDistinctFromDifferentServices()
    {
        var t = await _accessTokenSource.GetAccessToken();
        t.ShouldNotBeNullOrWhiteSpace();

        // If the access token source don't bleed into each other, the invalid source should throw an exception.
        await Assert.ThrowsAsync<PinMeToException>(
            () => _invalidAccessTokenSource.GetAccessToken()
        );
    }
}
