using System.Collections.Specialized;
using Microsoft.Extensions.Options;

namespace TheCodePatch.PinMeTo.Client.Clients;

internal class UrlFactory : IUrlFactory
{
    private readonly CurrentOptionsProvider _currentOptionsProvider;

    public UrlFactory(CurrentOptionsProvider currentOptionsProvider)
    {
        _currentOptionsProvider = currentOptionsProvider;
    }

    public string CreateRelativeUrl<TCustomData>(
        string relativePath,
        params (string name, string value)[] queryParameters
    )
    {
        var accountId = _currentOptionsProvider.GetCurrentOptions<TCustomData>().AccountId;
        var accountIdEsc = Uri.EscapeDataString(accountId);
        var url = $"/v2/{accountIdEsc}/{relativePath.TrimStart('/')}";

        if (queryParameters.Length > 0)
        {
            var parameters = queryParameters.Select(
                k => $"{k.name}={Uri.EscapeDataString(k.value)}"
            );
            url += "?" + string.Join("&", parameters);
        }

        return url;
    }
}
