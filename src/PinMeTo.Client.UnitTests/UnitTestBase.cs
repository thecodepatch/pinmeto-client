using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheCodePatch.PinMeTo.Client.UnitTests.Locations;
using Xunit.Abstractions;

// ReSharper disable VirtualMemberCallInConstructor

// Turn off all parallelization because the tests interfere with each other if it is activated.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace TheCodePatch.PinMeTo.Client.UnitTests;

/// <summary>
/// Base class for unit tests. Sets up common functionality
/// including logging and configuration management as well
/// as a service provider with the PinMeTo client already
/// bootstrapped.
/// </summary>
public abstract class UnitTestBase : IAsyncLifetime
{
    public async Task DisposeAsync()
    {
        await ((ServiceProvider)ServiceProvider).DisposeAsync();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    protected IServiceProvider ServiceProvider { get; }

    protected UnitTestBase(ITestOutputHelper testOutputHelper)
    {
        var conf = GetConfiguration();

        var serviceCollection = new ServiceCollection()
            // Bootstrap the PinMeTwo client library.
            .AddPinMeToClient<TestCustomData>(conf.GetSection("PinMeToClient"))
            .AddPinMeToClient<InvalidInstanceTestCustomData>(
                conf.GetSection("InvalidPinMeToClient")
            )
            // Redirect ILogger logging to the XUnit output.
            .AddLogging(l => l.SetMinimumLevel(LogLevel.Debug).AddXunit(testOutputHelper))
            // Get options for unit tests from configuration files.
            .AddOptions<UnitTestsOptions>()
            .Bind(conf.GetRequiredSection("UnitTestOptions"))
            .ValidateDataAnnotations()
            .Services;

        // Enable custom service configuration in the test classes.
        ConfigureServices(serviceCollection, conf);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    protected virtual IServiceCollection ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }

    private static string? GetBinPath()
    {
        var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
        var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
        return Path.GetDirectoryName(codeBasePath);
    }

    private static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(GetBinPath())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.private.json", true)
            .Build();
    }
}
