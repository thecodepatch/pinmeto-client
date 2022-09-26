using System.Collections.Generic;
using System.Web;
using TheCodePatch.PinMeTo.Client.Response;

namespace TheCodePatch.PinMeTo.Client;

public record PagedResult<TItems>
{
    public List<TItems> Items { get; init; } = null!;
    public PageNavigation? NextPage { get; init; }
    public PageNavigation? PreviousPage { get; init; }

    /// <summary>
    /// Ctor that maps a deserialized PagedResponse to a PagedResult.
    /// </summary>
    /// <param name="response">The deserialized response.</param>
    internal PagedResult(PagedResponse<TItems> response)
    {
        Items = response.Data;
        NextPage = CreatePageNavigation(PageNavigationDirection.Next, response.Paging.Next, "next");
        PreviousPage = CreatePageNavigation(
            PageNavigationDirection.Previous,
            response.Paging.Before,
            "before"
        );

        PageNavigation? CreatePageNavigation(
            PageNavigationDirection direction,
            string url,
            string parameterName
        )
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);
            return new(
                Convert.ToUInt32(query.Get("pagesize")),
                direction,
                query.Get(parameterName)!
            );
        }
    }
}
