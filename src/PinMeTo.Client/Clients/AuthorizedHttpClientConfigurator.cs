using System.Collections.Generic;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TheCodePatch.PinMeTo.Client.AccessToken;

namespace TheCodePatch.PinMeTo.Client.Clients;

/// <summary>
/// Functionality used to configure the authorized HTTP client.
///
/// The authorized HTTP client includes the access token in all requests, and is thereby authorized
/// to access resources in the parts of the API where an access token is required.
/// </summary>
internal static class AuthorizedHttpClientConfigurator
{
    /// <summary>
    /// Adds the authorized HTTP client to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="clientName">The name of the named HTTP client.</param>
    /// <returns>The service collection with the authorized HTTP client added.</returns>
    public static IServiceCollection AddAndConfigureAuthorizedHttpClient(
        this IServiceCollection services,
        out string clientName
    )
    {
        clientName = ClientName;
        services.TryAddTransient<LoggingDelegatingHttpClientHandler>();

        return services
            .AddTransient<AuthorizingDelegatingHttpHandler>()
            .AddHttpClient(ClientName)
            .ConfigureHttpClient(
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptionsMonitor<PinMeToClientOptions>>();
                    client.BaseAddress = options.CurrentValue.ApiBaseAddress;
                }
            )
            .ConfigureHttpMessageHandlerBuilder(
                b =>
                {
                    b.AdditionalHandlers.Add(
                        b.Services.GetRequiredService<AuthorizingDelegatingHttpHandler>()
                    );

                    b.AdditionalHandlers.Add(
                        b.Services.GetRequiredService<LoggingDelegatingHttpClientHandler>()
                    );
                }
            )
            .Services;
    }

    /// <summary>
    /// The name of the named HTTP client.
    /// </summary>
    private const string ClientName = "AuthorizedHttpClient";

    /// <summary>
    ///     Handler for the authorized HTTP client.
    ///     Adds the access token to the request headers.
    /// </summary>
    private class AuthorizingDelegatingHttpHandler : DelegatingHandler
    {
        private readonly IAccessTokenSource _accessTokenSource;

        public AuthorizingDelegatingHttpHandler(IAccessTokenSource accessTokenSource)
        {
            _accessTokenSource = accessTokenSource;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            var accessToken = await _accessTokenSource.GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
