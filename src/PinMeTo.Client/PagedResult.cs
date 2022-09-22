using System.Collections.Generic;
using System.Web;
using TheCodePatch.PinMeTo.Client.Response;

namespace TheCodePatch.PinMeTo.Client;

public record PagedResult<TItems>
{
    public List<TItems> Items { get; init; } = null!;
    public PageNavigation? NextPage { get; init; }
    public PageNavigation? PreviousPage { get; init; }

    internal PagedResult(PagedResponse<TItems> response)
    {
        Items = response.Data;
        NextPage = CreateChangePage(PageNavigationDirection.Next, response.Paging.Next, "next");
        PreviousPage = CreateChangePage(
            PageNavigationDirection.Previous,
            response.Paging.Before,
            "before"
        );

        PageNavigation? CreateChangePage(
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
