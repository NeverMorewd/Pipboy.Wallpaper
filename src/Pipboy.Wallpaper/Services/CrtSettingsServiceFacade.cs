using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipboy.Wallpaper.Abstractions;
using Pipboy.Wallpaper.Framework;
using Pipboy.Wallpaper.Models;
using System.IO;
using System.Text.Json;

namespace Pipboy.Wallpaper.Services;

public class CrtSettingsServiceFacade : ICrtSettingsServiceFacade
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };
    private static readonly string _configFile = Path.Combine(AppDataContext.Current.AppTempDirectory, "crt_config.json");
    private readonly ILogger _logger;

    public CrtSettingsServiceFacade(CrtDataContext data,
        TextDataContext textDataContext,
        INoiseSettingsService noiseSettingsService,
        IScanBeamSettingsService scanBeamSettingsService,
        IScanlineSettingsService scanlineSettingsService,
        IOptionsMonitor<CrtOptionsDto> crtOptionsMonitor,
        ILogger<CrtSettingsServiceFacade> logger)
    {
        _logger = logger;
        Data = data;
        TextData = textDataContext;
        Noise = noiseSettingsService;
        ScanBeam = scanBeamSettingsService;
        Scanline = scanlineSettingsService;

        crtOptionsMonitor.OnChange(_ => 
        {
            
        });

       // _ = HandleConfigAsync();
    }
    public CrtDataContext Data { get; }
    public TextDataContext TextData { get; }
    public INoiseSettingsService Noise { get; }
    public IScanBeamSettingsService ScanBeam { get; }
    public IScanlineSettingsService Scanline { get; }
    private async Task HandleConfigAsync()
    {
        if (!File.Exists(_configFile))
        {
            _logger.LogInformation("Crt config file not found, creating default config file.");
            await SaveToJsonAsync(_configFile);
        }
        else
        {
            _logger.LogInformation("Crt config file found, loading config.");
            await LoadFromJsonAsync(_configFile);
        }
    }
    public CrtOptionsDto ExportConfig() =>
        new()
        {
            EnableNoise = Data.EnableNoise,
            NoiseDensity = Data.NoiseDensity,
            NoiseOpacity = Data.NoiseOpacity,
            NoisePixelSize = Data.NoisePixelSize,
            NoiseRefreshRate = Data.NoiseRefreshRate,

            EnableScanBeamBlur = Data.EnableScanBeamBlur,
            ScanBeamColor = Data.ScanBeamColor,
            ScanBeamHeight = Data.ScanBeamHeight,

            EnableScanlineAnimation = Data.EnableScanlineAnimation,
            ScanlineAnimRefreshRate = Data.ScanlineAnimRefreshRate,
            ScanlineAnimSpeed = Data.ScanlineAnimSpeed,

            ScanlineColor = Data.ScanlineColor,
            ScanlineHeight = Data.ScanlineHeight,
            ScanlineSpacing = Data.ScanlineSpacing,

            UseCachedRendering = Data.UseCachedRendering
        };

    public void ApplyConfig(CrtOptionsDto c)
    {
        Data.EnableNoise = c.EnableNoise;
        Data.NoiseDensity = c.NoiseDensity;
        Data.NoiseOpacity = c.NoiseOpacity;
        Data.NoisePixelSize = c.NoisePixelSize;
        Data.NoiseRefreshRate = c.NoiseRefreshRate;

        Data.EnableScanBeamBlur = c.EnableScanBeamBlur;
        Data.ScanBeamColor = c.ScanBeamColor;
        Data.ScanBeamHeight = c.ScanBeamHeight;

        Data.EnableScanlineAnimation = c.EnableScanlineAnimation;
        Data.ScanlineAnimRefreshRate = c.ScanlineAnimRefreshRate;
        Data.ScanlineAnimSpeed = c.ScanlineAnimSpeed;

        Data.ScanlineColor = c.ScanlineColor;
        Data.ScanlineHeight = c.ScanlineHeight;
        Data.ScanlineSpacing = c.ScanlineSpacing;

        Data.UseCachedRendering = c.UseCachedRendering;
    }


    public Task SaveToJsonAsync(string path)
    {
        var dto = ExportConfig();
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        return File.WriteAllTextAsync(path, json);
    }

    public async Task LoadFromJsonAsync(string path)
    {
        if (!File.Exists(path)) return;

        var json = await File.ReadAllTextAsync(path);
        var dto = JsonSerializer.Deserialize<CrtOptionsDto>(json);

        if (dto != null)
            ApplyConfig(dto);
    }
}
