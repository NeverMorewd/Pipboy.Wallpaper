using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipboy.Wallpaper.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Windows.Media;

namespace Pipboy.Wallpaper.ViewModels;

public partial class EffectViewModel : ReactiveObject
{
    private readonly IOptionsMonitor<EffectOptionsModel> _effectOptionsMonitor;
    private readonly ILogger _logger;
    public EffectViewModel(IOptionsMonitor<EffectOptionsModel> effectOptionsMonitor, ILogger<EffectViewModel> logger)
    {
        _effectOptionsMonitor = effectOptionsMonitor;
        _logger = logger;
        UpdateFromOptions(_effectOptionsMonitor.CurrentValue);
        effectOptionsMonitor.OnChange(options =>
        {
            _logger.LogInformation("effect options changed, updating EffectDataContext.");
            UpdateFromOptions(options);
        });
    }

    private void UpdateFromOptions(EffectOptionsModel currentValue)
    {
        EnableNoise = currentValue.EnableNoise;
        EnableScanBeamBlur = currentValue.EnableScanBeamBlur;
        EnableScanlineAnimation = currentValue.EnableScanlineAnimation;
        UseCachedRendering = currentValue.UseCachedRendering;

        NoiseDensity = currentValue.NoiseDensity;
        NoiseOpacity = currentValue.NoiseOpacity;
        NoisePixelSize = currentValue.NoisePixelSize;
        NoiseRefreshRate = currentValue.NoiseRefreshRate;

        ScanBeamColor = currentValue.ScanBeamColor;
        ScanBeamHeight = currentValue.ScanBeamHeight;

        ScanlineAnimRefreshRate = currentValue.ScanlineAnimRefreshRate;
        ScanlineAnimSpeed = currentValue.ScanlineAnimSpeed;
        ScanlineColor = currentValue.ScanlineColor;
        ScanlineHeight = currentValue.ScanlineHeight;
        ScanlineSpacing = currentValue.ScanlineSpacing;
    }

    [Reactive] 
    private bool _enableNoise = true;
    [Reactive] 
    private bool _enableScanBeamBlur = true;
    [Reactive] 
    private bool _enableScanlineAnimation = true;

    [Reactive] 
    private double _noiseDensity = 0.02;
    [Reactive] 
    private double _noiseOpacity = 0.3;
    [Reactive] 
    private int _noisePixelSize = 3;
    [Reactive] 
    private int _noiseRefreshRate = 50;

    [Reactive] 
    private Color _scanBeamColor = Color.FromArgb(80, 0, 255, 0);
    [Reactive] 
    private double _scanBeamHeight = 100;

    [Reactive]
    private int _scanlineAnimRefreshRate = 16;
    [Reactive] 
    private double _scanlineAnimSpeed = 100;

    [Reactive] 
    private Color _scanlineColor = Color.FromArgb(35, 0, 255, 0);
    [Reactive] 
    private int _scanlineHeight = 5;
    [Reactive] 
    private int _scanlineSpacing = 15;

    [Reactive] 
    private bool _useCachedRendering = true;
}
