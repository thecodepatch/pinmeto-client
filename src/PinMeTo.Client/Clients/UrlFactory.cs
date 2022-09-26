using System.Collections.Specialized;
using Microsoft.Extensions.Options;

namespace TheCodePatch.PinMeTo.Client.Clients;

internal class UrlFactory : IUrlFactory
{
    private readonly IOptionsMonitor<PinMeToClientOptions> _options;

    public UrlFactory(IOptionsMonitor<PinMeToClientOptions> options)
    {
        _options = options;
    }

    public string CreateRelativeUrl(
        string relativePath,
        params (string name, string value)[] queryParameters
    )
    {
        var accountIdEsc = Uri.EscapeDataString(_options.CurrentValue.AccountId);
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
