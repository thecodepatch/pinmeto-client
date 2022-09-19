using System.Collections.Generic;
using System.Reflection;
using Divergic.Logging.Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

// ReSharper disable VirtualMemberCallInConstructor

namespace TheCodePatch.PinMeToClient.UnitTests;

public abstract class UnitTestBase
{
    protected IServiceProvider ServiceProvider { get; }

    protected UnitTestBase(ITestOutputHelper testOutputHelper)
    {
        var conf = GetConfiguration();

        var loggingConfig = new LoggingConfig();
        conf.GetSection("Logging").Bind(loggingConfig);

        var serviceCollection = new ServiceCollection()
            // Bootstrap the PinMeTwo client library.
            .AddPinMeToClient(conf.GetSection("PinMeToClient"))
            // Redirect ILogger logging to the XUnit output.
            .AddLogging(
                l =>
                {
                    l.AddXunit(testOutputHelper);
                    l.SetMinimumLevel(LogLevel.Debug);
                }
            );

        // Enable custom service configuration in the test classes.
        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    protected virtual IServiceCollection ConfigureServices(IServiceCollection services)
    {
        return services;
    }

    private static string? GetBinPath()
    {
        var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
        var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
        return Path.GetDirectoryName(codeBasePath);
    }

    private IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(GetBinPath())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.private.json", true)
            .Build();
    }
}
