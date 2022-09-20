using System.Collections.Specialized;
using Microsoft.Extensions.Options;
using TheCodePatch.PinMeToClient.AccessToken;
using TheCodePatch.PinMeToClient.Exceptions;
using TheCodePatch.PinMeToClient.Response;

namespace TheCodePatch.PinMeToClient.Locations;

internal class LocationsClient : ILocationsClient
{
    private readonly IAccessTokenSource _accessTokenSource;
    private readonly HttpClient _client;
    private readonly IOptionsMonitor<PinMeToClientOptions> _options;
    private readonly IResponseHandler _responseHandler;

    public LocationsClient(
        HttpClient client,
        IOptionsMonitor<PinMeToClientOptions> options,
        IAccessTokenSource accessTokenSource,
        IResponseHandler responseHandler
    )
    {
        _client = client;
        _options = options;
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

    public async Task<PagedResult<Location>> List(PageNavigation changePage)
    {
        Guard.IsWithinRange(nameof(changePage.PageSize), changePage.PageSize, 0, 250);

        var urlParameters = new NameValueCollection
        {
            { "pagesize", changePage.PageSize.ToString() }
        };

        if (null != changePage.Key)
        {
            var direction = changePage.Direction switch
            {
                PageNavigationDirection.Next => "next",
                PageNavigationDirection.Previous => "before",
                _ => throw new ArgumentOutOfRangeException(),
            };

            urlParameters.Add(direction, changePage.Key);
        }

        var url = await GetUrl(null, urlParameters);

        var response = await _client.GetAsync(url);
        var locations = await _responseHandler.DeserializeOrThrow<PagedResponse<Location>>(
            response
        );

        return new PagedResult<Location>(locations);
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
