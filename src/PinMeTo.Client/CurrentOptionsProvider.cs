using Microsoft.Extensions.Options;

namespace TheCodePatch.PinMeTo.Client;

internal class CurrentOptionsProvider
{
    private readonly IOptionsMonitor<PinMeToClientOptions> _optionsMonitor;

    public CurrentOptionsProvider(IOptionsMonitor<PinMeToClientOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public PinMeToClientOptions GetCurrentOptions<TCustomData>()
    {
        var optionsName = CreateOptionsName<TCustomData>();
        return _optionsMonitor.Get(optionsName);
    }

    public static string CreateOptionsName<TCustomData>() =>
        $"PinMeToOptions-{typeof(TCustomData).FullName}";
}
