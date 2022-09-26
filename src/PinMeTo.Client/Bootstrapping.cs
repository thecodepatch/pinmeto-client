using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TheCodePatch.PinMeTo.Client.AccessToken;
using TheCodePatch.PinMeTo.Client.Clients;
using TheCodePatch.PinMeTo.Client.Locations;
using TheCodePatch.PinMeTo.Client.Response;
using TheCodePatch.PinMeTo.Client.Serialization;

namespace TheCodePatch.PinMeTo.Client;

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
            .AddSingleton<IUrlFactory, UrlFactory>()
            .AddSingleton<IResponseHandler, ResponseHandler>()
            // Add the http client authenticated to communicate with the PinMeTo API.
            .AddAndConfigureAuthenticatedHttpClient()
            .AddSingleton<IAccessTokenSource, AccessTokenSource>()
            // Add the http client authorized to use resources provided by the PinMeTo API.
            .AddAndConfigureAuthorizedHttpClient(out var authorizedHttpClientName)
            // Add the service providing access to the Locations resources in the API.
            .AddHttpClient<ILocationsService, LocationsService>(authorizedHttpClientName)
            .Services;
    }
}
