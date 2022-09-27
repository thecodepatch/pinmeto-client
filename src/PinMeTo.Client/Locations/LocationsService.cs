using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using TheCodePatch.PinMeTo.Client.Clients;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Locations.Model;
using TheCodePatch.PinMeTo.Client.Response;
using TheCodePatch.PinMeTo.Client.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations;

internal class LocationsService<TCustomData> : ILocationsService<TCustomData>
{
    private readonly HttpClient _client;
    private readonly ISerializer _serializer;
    private readonly IResponseHandler _responseHandler;
    private readonly IUrlFactory _urlFactory;
    private readonly ILogger<LocationsService<TCustomData>> _logger;

    public LocationsService(
        HttpClient client,
        ISerializer serializer,
        IResponseHandler responseHandler,
        IUrlFactory urlFactory,
        ILogger<LocationsService<TCustomData>> logger
    )
    {
        _client = client;
        _serializer = serializer;
        _responseHandler = responseHandler;
        _urlFactory = urlFactory;
        _logger = logger;
    }

    public async Task<PinMeToResult<LocationDetails<TCustomData>>> Get(string storeId)
    {
        Guard.IsNotNullOrWhiteSpace(nameof(storeId), storeId);
        var url = _urlFactory.CreateRelativeUrl($"locations/{storeId}");
        var response = await _client.GetAsync(url);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails<TCustomData>>>(
            response
        );
        return WrapInResult(response, result.Data);
    }

    public async Task<PinMeToResult<LocationDetails<TCustomData>>> Create(CreateLocationInput<TCustomData> input)
    {
        var url = _urlFactory.CreateRelativeUrl("locations");
        var content = _serializer.MakeJson(input);
        var response = await _client.PostAsync(url, content);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails<TCustomData>>>(
            response
        );
        return WrapInResult(response, result.Data);
    }

    public async Task<PinMeToResult<LocationDetails<TCustomData>>> CreateOrUpdate(CreateLocationInput<TCustomData> input)
    {
        var url = _urlFactory.CreateRelativeUrl("locations", ("upsert", "true"));
        var content = _serializer.MakeJson(input);
        var response = await _client.PostAsync(url, content);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails<TCustomData>>>(
            response
        );
        return WrapInResult(response, result.Data);
    }

    public async Task<PinMeToResult<PagedResult<Location<TCustomData>>>> List(PageNavigation pageNavigation)
    {
        Guard.IsWithinRange(nameof(pageNavigation.PageSize), pageNavigation.PageSize, 0, 250);

        var urlParameters = new List<(string name, string value)>()
        {
            new("pagesize", pageNavigation.PageSize.ToString()),
        };

        if (null != pageNavigation.Key)
        {
            var direction = pageNavigation.Direction switch
            {
                PageNavigationDirection.Next => "next",
                PageNavigationDirection.Previous => "before",
                _ => throw new ArgumentOutOfRangeException(nameof(pageNavigation)),
            };

            urlParameters.Add(new(direction, pageNavigation.Key));
        }

        var url = _urlFactory.CreateRelativeUrl("locations", urlParameters.ToArray());

        var response = await _client.GetAsync(url);
        var locations = await _responseHandler.DeserializeOrThrow<PagedResponse<Location<TCustomData>>>(
            response
        );

        return WrapInResult(response, new PagedResult<Location<TCustomData>>(locations));
    }

    public async Task<PinMeToResult<LocationDetails<TCustomData>>> UpdateLocation(
        string storeId,
        UpdateLocationInput<TCustomData> input
    )
    {
        var url = _urlFactory.CreateRelativeUrl($"locations/{storeId}");
        var content = _serializer.MakeJson(input);
        var response = await _client.PutAsync(url, content);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails<TCustomData>>>(
            response
        );
        return WrapInResult(response, result.Data);
    }

    private PinMeToResult<T> WrapInResult<T>(HttpResponseMessage response, T data)
    {
        var limit = ReadHeader("limit");
        var reset = ReadHeader("reset");
        var remaining = ReadHeader("remaining");

        var rateLimit =
            limit.HasValue && reset.HasValue && remaining.HasValue
                ? new RateLimit(limit.Value, reset.Value, remaining.Value)
                : null;

        _logger.LogDebug("Rate limit: {@RateLimit}", rateLimit);

        return new() { Result = data, RateLimit = rateLimit, };

        int? ReadHeader(string name)
        {
            return response.Headers.TryGetValues($"x-ratelimit-{name}", out var values)
              ? int.TryParse(values.FirstOrDefault(), out var v)
                  ? v
                  : null
              : null;
        }
    }
}
