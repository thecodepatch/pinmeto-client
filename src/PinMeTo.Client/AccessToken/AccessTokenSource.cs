using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheCodePatch.PinMeTo.Client.Clients;
using TheCodePatch.PinMeTo.Client.Response;

namespace TheCodePatch.PinMeTo.Client.AccessToken;

internal class AccessTokenSource<TCustomData> : IAccessTokenSource<TCustomData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AccessTokenSource<TCustomData>> _logger;
    private readonly CurrentOptionsProvider _optionsProvider;
    private readonly IResponseHandler _responseHandler;
    private AccessToken? _currentToken;

    public AccessTokenSource(
        IHttpClientFactory httpClientFactory,
        CurrentOptionsProvider optionsProvider,
        IResponseHandler responseHandler,
        ILogger<AccessTokenSource<TCustomData>> logger
    )
    {
        _httpClientFactory = httpClientFactory;
        _optionsProvider = optionsProvider;
        _responseHandler = responseHandler;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> GetAccessToken()
    {
        if (_currentToken?.GetRemainingValiditySeconds() > 10)
        {
            _logger.LogDebug("Using previously retrieved access token that is still valid");
            return _currentToken.Value;
        }

        _logger.LogDebug(
            "Previously retrieved access token was {PrevTokenStatus}",
            null == _currentToken ? "not found" : "expired"
        );

        await RefreshToken();
        return await GetAccessToken();
    }

    private async Task RefreshToken()
    {
        var deserialized = await RetrieveTokenFromApi();

        _logger.LogDebug(
            "New access token will be used for {ValidityDuration} seconds",
            deserialized.ExpiresInSeconds
        );

        _currentToken = new AccessToken
        {
            Expires = DateTime.Now.AddSeconds(deserialized.ExpiresInSeconds),
            Value = deserialized.Value,
        };
    }

    private async Task<ResponseModel> RetrieveTokenFromApi()
    {
        var options = _optionsProvider.GetCurrentOptions<TCustomData>();
        var message = new Dictionary<string, string>
        {
            { "client_id", options.AppId },
            { "client_secret", options.AppSecret },
            { "grant_type", "client_credentials" },
        };
        var client = _httpClientFactory.CreateClient(
            AuthenticatedHttpClientConfigurator.GetAuthenticatedHttpClientName<TCustomData>()
        );
        const string url = "/oauth/token";
        var response = await client.PostAsync(url, new FormUrlEncodedContent(message));
        var result = await _responseHandler.DeserializeOrThrow<ResponseModel>(
            url,
            message,
            response
        );
        return result;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    // ReSharper disable once UnusedMember.Local
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    private record ResponseModel
    {
        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds { get; init; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; } = null!;

        [JsonPropertyName("access_token")]
        public string Value { get; init; } = null!;
    }
}
