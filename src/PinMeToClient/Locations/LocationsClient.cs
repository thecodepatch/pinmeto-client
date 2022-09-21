using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using TheCodePatch.PinMeToClient.AccessToken;
using TheCodePatch.PinMeToClient.Exceptions;
using TheCodePatch.PinMeToClient.Locations.Model;
using TheCodePatch.PinMeToClient.Response;
using TheCodePatch.PinMeToClient.Serialization;

namespace TheCodePatch.PinMeToClient.Locations;

internal class LocationsClient : ILocationsClient
{
    private readonly IAccessTokenSource _accessTokenSource;
    private readonly HttpClient _client;
    private readonly IOptionsMonitor<PinMeToClientOptions> _options;
    private readonly ISerializer _serializer;
    private readonly IResponseHandler _responseHandler;

    public LocationsClient(
        HttpClient client,
        IOptionsMonitor<PinMeToClientOptions> options,
        ISerializer serializer,
        IAccessTokenSource accessTokenSource,
        IResponseHandler responseHandler
    )
    {
        _client = client;
        _options = options;
        _serializer = serializer;
        _accessTokenSource = accessTokenSource;
        _responseHandler = responseHandler;
        _client.BaseAddress = options.CurrentValue.ApiBaseAddress;
    }

    public async Task<LocationDetails> Get(string storeId)
    {
        Guard.IsNotNullOrWhiteSpace(nameof(storeId), storeId);
        var url = await GetUrl($"/{storeId}/");

        var response = await _client.GetAsync(url);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails>>(
            response
        );
        return result.Data;
    }

    public async Task<LocationDetails> Create(CreateLocationInput input)
    {
        var url = await GetUrl(null);
        var content = _serializer.MakeJson(input);
        var response = await _client.PostAsync(url, content);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails>>(
            response
        );
        return result.Data;
    }

    public async Task<LocationDetails> CreateOrUpdate(CreateLocationInput input)
    {
        var parameters = new NameValueCollection { { "upsert", "true" } };
        var url = await GetUrl(null, parameters);
        var content = _serializer.MakeJson(input);

        var response = await _client.PostAsync(url, content);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails>>(
            response
        );
        return result.Data;
    }

    public async Task<PagedResult<Location>> List(PageNavigation pageNavigation)
    {
        Guard.IsWithinRange(nameof(pageNavigation.PageSize), pageNavigation.PageSize, 0, 250);

        var urlParameters = new NameValueCollection
        {
            { "pagesize", pageNavigation.PageSize.ToString() },
        };

        if (null != pageNavigation.Key)
        {
            var direction = pageNavigation.Direction switch
            {
                PageNavigationDirection.Next => "next",
                PageNavigationDirection.Previous => "before",
                _ => throw new ArgumentOutOfRangeException(),
            };

            urlParameters.Add(direction, pageNavigation.Key);
        }

        var url = await GetUrl(null, urlParameters);

        var response = await _client.GetAsync(url);
        var locations = await _responseHandler.DeserializeOrThrow<PagedResponse<Location>>(
            response
        );

        return new PagedResult<Location>(locations);
    }

    public async Task<LocationDetails> UpdateLocation(string storeId, UpdateLocationInput input)
    {
        var url = await GetUrl($"/{storeId}/");
        var content = _serializer.MakeJson(input);
        var response = await _client.PutAsync(url, content);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails>>(
            response
        );
        return result.Data;
    }

    private async Task<string> GetUrl(string? path, NameValueCollection? queryParameters = null)
    {
        var accessToken = await _accessTokenSource.GetAccessToken();
        var accountIdEsc = Uri.EscapeDataString(_options.CurrentValue.AccountId);
        var accessTokenEsc = Uri.EscapeDataString(accessToken);
        var url = $"/v2/{accountIdEsc}/locations{path}?access_token={accessTokenEsc}";

        if (null != queryParameters)
        {
            var parameters = queryParameters.AllKeys.Select(
                k => $"{k}={Uri.EscapeDataString(queryParameters.Get(k) ?? "")}"
            );
            url += "&" + string.Join("&", parameters);
        }

        return url;
    }
}
