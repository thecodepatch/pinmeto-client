namespace TheCodePatch.PinMeToClient.Locations;

public interface ILocationsClient
{
    Task<PagedResult<Location>> List(PageNavigation changePage);

    Task<LocationDetails> Get(string storeId);
}
