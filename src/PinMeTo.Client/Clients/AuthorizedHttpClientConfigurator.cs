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
    public static IServiceCollection AddAndConfigureAuthorizedHttpClient<TCustomData>(
        this IServiceCollection services,
        out string clientName
    )
    {
        clientName = $"PinMeToAuthorizedHttpClient-{typeof(TCustomData).FullName}";
        services.TryAddTransient<LoggingDelegatingHttpClientHandler>();

        return services
            .AddTransient<AuthorizingDelegatingHttpHandler<TCustomData>>()
            .AddHttpClient(clientName)
            .ConfigureHttpClient(
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<CurrentOptionsProvider>()
                        .GetCurrentOptions<TCustomData>();
                    client.BaseAddress = options.ApiBaseAddress;
                }
            )
            .ConfigureHttpMessageHandlerBuilder(b =>
            {
                b.AdditionalHandlers.Add(
                    b.Services.GetRequiredService<AuthorizingDelegatingHttpHandler<TCustomData>>()
                );

                b.AdditionalHandlers.Add(
                    b.Services.GetRequiredService<LoggingDelegatingHttpClientHandler>()
                );
            })
            .Services;
    }

    /// <summary>
    ///     Handler for the authorized HTTP client.
    ///     Adds the access token to the request headers.
    /// </summary>
    private class AuthorizingDelegatingHttpHandler<TCustomData> : DelegatingHandler
    {
        private readonly IAccessTokenSource<TCustomData> _accessTokenSource;

        public AuthorizingDelegatingHttpHandler(IAccessTokenSource<TCustomData> accessTokenSource)
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
