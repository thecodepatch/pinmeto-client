using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using TheCodePatch.PinMeTo.Client.Clients;
using TheCodePatch.PinMeTo.Client.Exceptions;
using TheCodePatch.PinMeTo.Client.Locations.Model;
using TheCodePatch.PinMeTo.Client.Response;
using TheCodePatch.PinMeTo.Client.Serialization;

namespace TheCodePatch.PinMeTo.Client.Locations;

internal class LocationsService : ILocationsService
{
    private readonly HttpClient _client;
    private readonly ISerializer _serializer;
    private readonly IResponseHandler _responseHandler;
    private readonly IUrlFactory _urlFactory;

    public LocationsService(
        HttpClient client,
        ISerializer serializer,
        IResponseHandler responseHandler,
        IUrlFactory urlFactory
    )
    {
        _client = client;
        _serializer = serializer;
        _responseHandler = responseHandler;
        _urlFactory = urlFactory;
    }

    public async Task<LocationDetails> Get(string storeId)
    {
        Guard.IsNotNullOrWhiteSpace(nameof(storeId), storeId);
        var url = _urlFactory.CreateRelativeUrl($"locations/{storeId}");
        var response = await _client.GetAsync(url);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails>>(
            response
        );
        return result.Data;
    }

    public async Task<LocationDetails> Create(CreateLocationInput input)
    {
        var url = _urlFactory.CreateRelativeUrl("locations");
        var content = _serializer.MakeJson(input);
        var response = await _client.PostAsync(url, content);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails>>(
            response
        );
        return result.Data;
    }

    public async Task<LocationDetails> CreateOrUpdate(CreateLocationInput input)
    {
        var url = _urlFactory.CreateRelativeUrl("locations", ("upsert", "true"));
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
        var locations = await _responseHandler.DeserializeOrThrow<PagedResponse<Location>>(
            response
        );

        return new PagedResult<Location>(locations);
    }

    public async Task<LocationDetails> UpdateLocation(string storeId, UpdateLocationInput input)
    {
        var url = _urlFactory.CreateRelativeUrl($"locations/{storeId}");
        var content = _serializer.MakeJson(input);
        var response = await _client.PutAsync(url, content);
        var result = await _responseHandler.DeserializeOrThrow<AtomicResponse<LocationDetails>>(
            response
        );
        return result.Data;
    }
}
