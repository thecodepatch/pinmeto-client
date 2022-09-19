using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheCodePatch.PinMeToClient.AccessToken;

namespace TheCodePatch.PinMeToClient;

public static class Bootstrapping
{
    /// <summary>
    ///     Adds the PinMeTo client implementation to the dependency container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configurator for the options of the client.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddPinMeToClient(
        this IServiceCollection services,
        Action<PinMeToClientOptions>? configure
    )
    {
        return services
            .AddPinMeToClientInternal()
            .AddOptions<PinMeToClientOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .Services;
    }

    public static IServiceCollection AddPinMeToClient(
        this IServiceCollection services,
        IConfigurationSection configurationSection
    )
    {
        return services
            .AddPinMeToClientInternal()
            .AddOptions<PinMeToClientOptions>()
            .Bind(configurationSection)
            .ValidateDataAnnotations()
            .Services;
    }

    private static IServiceCollection AddPinMeToClientInternal(this IServiceCollection services)
    {
        services.AddHttpClient<IPinMeToClient, PinMeToClient>();
        services.AddSingleton<IAccessTokenSource, AccessTokenSource>();
        return services;
    }
}
