using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TheCodePatch.PinMeTo.Client.Clients;

internal class LoggingDelegatingHttpClientHandler : DelegatingHandler
{
    private readonly ILogger<LoggingDelegatingHttpClientHandler> _logger;

    public LoggingDelegatingHttpClientHandler(ILogger<LoggingDelegatingHttpClientHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        if (_logger.IsEnabled(LogLevel.Debug) && null != request.Content)
        {
            var content = await request.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Submitting request: {RequestContent}", content);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
