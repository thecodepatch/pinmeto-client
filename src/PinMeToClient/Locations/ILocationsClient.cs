using TheCodePatch.PinMeToClient.Locations.Model;

namespace TheCodePatch.PinMeToClient.Locations;

public interface ILocationsClient
{
    /// <summary>
    /// Creates a new location.
    /// </summary>
    /// <param name="input">Input data for the location.</param>
    /// <returns>The created location.</returns>
    Task<LocationDetails> Create(CreateLocationInput input);

    /// <summary>
    /// Creates a new location. If the location already exists, it is instead updated.
    /// </summary>
    /// <param name="input">Input data for the location.</param>
    /// <returns>The updated location.</returns>
    Task<LocationDetails> CreateOrUpdate(CreateLocationInput input);

    /// <summary>
    /// Gets details for a location.
    /// </summary>
    /// <param name="storeId"></param>
    /// <returns></returns>
    Task<LocationDetails> Get(string storeId);

    /// <summary>
    /// Lists existing locations.
    /// </summary>
    /// <param name="pageNavigation">Control input for paging and page size.</param>
    /// <returns>A paged result with the items of the current page and navigation items to navigate to adjacent pages.</returns>
    Task<PagedResult<Location>> List(PageNavigation pageNavigation);

    /// <summary>
    /// Updates the specified location.
    /// </summary>
    /// <param name="storeId">The store id for the location to update.</param>
    /// <param name="input">The data to update.</param>
    /// <returns>The updated data.</returns>
    Task<LocationDetails> UpdateLocation(string storeId, UpdateLocationInput input);
}
