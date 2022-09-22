﻿using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Logging;
using TheCodePatch.PinMeToClient.AccessToken;
using TheCodePatch.PinMeToClient.Communication;
using TheCodePatch.PinMeToClient.Locations;
using TheCodePatch.PinMeToClient.Response;
using TheCodePatch.PinMeToClient.Serialization;

namespace TheCodePatch.PinMeToClient;

public static class Bootstrapping
{
    /// <summary>
    ///     Adds the PinMeTo client implementation to the dependency container with manual configuration of the options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configurator for the options of the client.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddPinMeToClient(
        this IServiceCollection services,
        Action<PinMeToClientOptions> configure
    )
    {
        return services
            .AddPinMeToClientInternal()
            .AddOptions<PinMeToClientOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .Services;
    }

    /// <summary>
    ///     Adds the PinMeTo client implementation to the dependency container with options from a configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurationSection">
    ///     A configuration section specifying properties that match
    ///     <see cref="PinMeToClientOptions" />.
    /// </param>
    /// <returns>The service collection.</returns>
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
        return services
            .AddSingleton<ISerializer, Serializer>()
            .AddSingleton<IAccessTokenSource, AccessTokenSource>()
            .AddSingleton<IResponseHandler, ResponseHandler>()
            .AddSingleton<LoggingHttpClientHandler>()
            .AddHttpClient<ILocationsClient, LocationsClient>()
            .ConfigureHttpMessageHandlerBuilder(
                b =>
                    b.AdditionalHandlers.Add(
                        b.Services.GetRequiredService<LoggingHttpClientHandler>()
                    )
            )
            .Services;
    }
}