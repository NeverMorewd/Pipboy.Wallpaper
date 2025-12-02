using Pipboy.Wallpaper.Abstractions;
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
    public CrtDataContext Data { get; }
    public INoiseSettingsService Noise { get; }
    public IScanBeamSettingsService ScanBeam { get; }
    public IScanlineSettingsService Scanline { get; }


    public CrtSettingsServiceFacade(CrtDataContext data, 
        INoiseSettingsService noiseSettingsService, 
        IScanBeamSettingsService scanBeamSettingsService, 
        IScanlineSettingsService scanlineSettingsService)
    {
        Data = data;
        Noise = noiseSettingsService;
        ScanBeam = scanBeamSettingsService;
        Scanline = scanlineSettingsService;
    }

    public CrtConfigDto ExportConfig() =>
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

    public void ApplyConfig(CrtConfigDto c)
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


    public void SaveToJson(string path)
    {
        var dto = ExportConfig();
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        File.WriteAllText(path, json);
    }

    public void LoadFromJson(string path)
    {
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);
        var dto = JsonSerializer.Deserialize<CrtConfigDto>(json);

        if (dto != null)
            ApplyConfig(dto);
    }
}
