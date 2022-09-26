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
    /// <summary>
    /// The name of the named client configured as an authenticated client.
    /// </summary>
    internal const string HttpClientName = "AccessTokenClient";

    /// <summary>
    /// Adds the authenticated HTTP client to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection with the authenticated HTTP client added.</returns>
    public static IServiceCollection AddAndConfigureAuthenticatedHttpClient(
        this IServiceCollection services
    )
    {
        services.TryAddTransient<LoggingDelegatingHttpClientHandler>();
        return services
            // Add logging http handler used by all clients.
            .AddHttpClient(
                HttpClientName,
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptionsMonitor<PinMeToClientOptions>>();
                    var credentials =
                        $"{options.CurrentValue.AppId}:{options.CurrentValue.AppSecret}";
                    var b64Credentials = Convert.ToBase64String(
                        Encoding.UTF8.GetBytes(credentials)
                    );

                    client.BaseAddress = options.CurrentValue.ApiBaseAddress;
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
