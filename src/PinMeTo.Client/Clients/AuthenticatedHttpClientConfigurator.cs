using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace TheCodePatch.PinMeTo.Client.Clients;

/// <summary>
/// Functionality used to configure the authenticated HTTP client.
///
/// The authenticated HTTP client is authenticated to communicate with the PinMeTo API but is not authorized
/// to use any of the resources except the one for requesting access tokens. Effectively, this client is only
/// used when requesting access tokens.
/// </summary>
internal static class AuthenticatedHttpClientConfigurator
{
    public static string GetAuthenticatedHttpClientName<TCustomData>() =>
        $"PinMeToAccessTokenClient-{typeof(TCustomData).FullName}";

    /// <summary>
    /// Adds the authenticated HTTP client to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection with the authenticated HTTP client added.</returns>
    public static IServiceCollection AddAndConfigureAuthenticatedHttpClient<TCustomData>(
        this IServiceCollection services
    )
    {
        services.TryAddTransient<LoggingDelegatingHttpClientHandler>();
        return services
            // Add logging http handler used by all clients.
            .AddHttpClient(
                GetAuthenticatedHttpClientName<TCustomData>(),
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<CurrentOptionsProvider>()
                        .GetCurrentOptions<TCustomData>();

                    var credentials = $"{options.AppId}:{options.AppSecret}";
                    var b64Credentials = Convert.ToBase64String(
                        Encoding.UTF8.GetBytes(credentials)
                    );

                    client.BaseAddress = options.ApiBaseAddress;
                    client.DefaultRequestHeaders.Authorization = new("Basic", b64Credentials);
                    client.Timeout = TimeSpan.FromSeconds(5);
                }
            )
            .ConfigureHttpMessageHandlerBuilder(
                b =>
                    b.AdditionalHandlers.Add(
                        b.Services.GetRequiredService<LoggingDelegatingHttpClientHandler>()
                    )
            )
            .Services;
    }
}
