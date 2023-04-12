using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public static IServiceCollection AddPinMeToClient<TCustomData>(
        this IServiceCollection services,
        Action<PinMeToClientOptions> configure
    )
    {
        return services
            .AddPinMeToClientInternal<TCustomData>()
            .AddPinMeToOptions<TCustomData>()
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
    public static IServiceCollection AddPinMeToClient<TCustomData>(
        this IServiceCollection services,
        IConfigurationSection configurationSection
    )
    {
        return services
            .AddPinMeToClientInternal<TCustomData>()
            .AddPinMeToOptions<TCustomData>()
            .Bind(configurationSection)
            .ValidateDataAnnotations()
            .Services;
    }

    private static OptionsBuilder<PinMeToClientOptions> AddPinMeToOptions<TCustomData>(
        this IServiceCollection services
    )
    {
        var optionsName = CurrentOptionsProvider.CreateOptionsName<TCustomData>();
        return services.AddOptions<PinMeToClientOptions>(optionsName);
    }

    private static IServiceCollection AddPinMeToClientInternal<TCustomData>(
        this IServiceCollection services
    )
    {
        services.TryAddSingleton<CurrentOptionsProvider>();
        services.TryAddSingleton<ISerializer, Serializer>();
        services.TryAddSingleton<IUrlFactory, UrlFactory>();
        services.TryAddSingleton<IExceptionFactory, ExceptionFactory>();
        services.TryAddSingleton<IResponseHandler, ResponseHandler>();

        return services
            // Add the http client authenticated to communicate with the PinMeTo API.
            .AddAndConfigureAuthenticatedHttpClient<TCustomData>()
            .AddSingleton<IAccessTokenSource, AccessTokenSource>()
            // Add the http client authorized to use resources provided by the PinMeTo API.
            .AddAndConfigureAuthorizedHttpClient<TCustomData>(out var httpClientName)
            // Add the service providing access to the Locations resources in the API.
            .AddHttpClient<ILocationsService<TCustomData>, LocationsService<TCustomData>>(
                httpClientName
            )
            .Services;
    }
}
