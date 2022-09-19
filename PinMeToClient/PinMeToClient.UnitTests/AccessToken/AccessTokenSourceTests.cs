using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheCodePatch.PinMeToClient.AccessToken;
using Xunit.Abstractions;

namespace TheCodePatch.PinMeToClient.UnitTests.AccessToken;

public class AccessTokenSourceTests : UnitTestBase
{
    private readonly IAccessTokenSource _accessTokenSource;

    public AccessTokenSourceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _accessTokenSource = ServiceProvider.GetRequiredService<IAccessTokenSource>();
    }

    [Fact]
    public async Task TokenIsRetrievedWithExpectedValues()
    {
        var t = await _accessTokenSource.GetAccessToken();
        t.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ConsecutiveTokenRetrievalsAreFast()
    {
        await _accessTokenSource.GetAccessToken();

        var s = Stopwatch.StartNew();
        for (int i = 0; i < 10; i++)
        {
            await _accessTokenSource.GetAccessToken();
        }

        s.ElapsedMilliseconds.ShouldBeLessThan(5);
    }
}
