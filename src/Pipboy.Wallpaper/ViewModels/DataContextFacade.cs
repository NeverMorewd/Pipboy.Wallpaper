using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipboy.Wallpaper.Abstractions;
using Pipboy.Wallpaper.Models;
using Pipboy.Wallpaper.ViewModels;
using System.Windows.Media;

namespace Pipboy.Wallpaper.Services;

public class DataContextFacade : IDataContextFacade
{
   
    private readonly ILogger _logger;

    public DataContextFacade(EffectViewModel effectViewModel,
        TextViewModel textDataContext,
        IOptionsMonitor<SystemOptionsModel> optionsMonitor,
        ILogger<DataContextFacade> logger)
    {
        _logger = logger;
        EffectData = effectViewModel;
        TextData = textDataContext;
        InitAndWatchSystemOptions(optionsMonitor);
    }
    public EffectViewModel EffectData { get; }
    public TextViewModel TextData { get; }

    private void InitAndWatchSystemOptions(IOptionsMonitor<SystemOptionsModel> optionsMonitor)
    {
        var currentOptions = optionsMonitor.CurrentValue;
        SetRenderingMode(currentOptions.EnableGpuAcceleration);
        optionsMonitor.OnChange(options =>
        {
            _logger.LogInformation("system options changed, applying new settings.");
        });
    }
    private static void SetRenderingMode(bool useGpuRendering)
    {
        if (useGpuRendering)
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
        }
        else
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
        }
    }
}
