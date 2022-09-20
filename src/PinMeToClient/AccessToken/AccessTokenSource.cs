using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheCodePatch.PinMeToClient.Response;

namespace TheCodePatch.PinMeToClient.AccessToken;

internal class AccessTokenSource : IAccessTokenSource
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AccessTokenSource> _logger;
    private readonly IOptionsMonitor<PinMeToClientOptions> _options;
    private readonly IResponseHandler _responseHandler;
    private AccessToken? _currentToken;

    public AccessTokenSource(
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<PinMeToClientOptions> options,
        IResponseHandler responseHandler,
        ILogger<AccessTokenSource> logger
    )
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
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

    private HttpClient CreateAndSetupClient()
    {
        var client = _httpClientFactory.CreateClient();
        var credentials = $"{_options.CurrentValue.AppId}:{_options.CurrentValue.AppSecret}";
        var b64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

        client.BaseAddress = _options.CurrentValue.ApiBaseAddress;
        client.DefaultRequestHeaders.Authorization = new("Basic", b64Credentials);
        client.Timeout = TimeSpan.FromSeconds(5);
        return client;
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
        var message = new Dictionary<string, string>
        {
            { "client_id", _options.CurrentValue.AppId },
            { "client_secret", _options.CurrentValue.AppSecret },
            { "grant_type", "client_credentials" },
        };
        var client = CreateAndSetupClient();
        var response = await client.PostAsync("/oauth/token", new FormUrlEncodedContent(message));
        return await _responseHandler.DeserializeOrThrow<ResponseModel>(response);
    }

    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    private record ResponseModel
    {
        [JsonPropertyName("expires_in")] public int ExpiresInSeconds { get; init; }

        [JsonPropertyName("token_type")] public string TokenType { get; init; } = null!;

        [JsonPropertyName("access_token")] public string Value { get; init; } = null!;
    }
}
