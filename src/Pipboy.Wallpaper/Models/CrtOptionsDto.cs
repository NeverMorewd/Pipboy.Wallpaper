using Pipboy.Wallpaper.Utils;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Pipboy.Wallpaper.Models;

public class CrtOptionsDto
{
    public bool EnableNoise { get; set; }
    public double NoiseDensity { get; set; }
    public double NoiseOpacity { get; set; }
    public int NoisePixelSize { get; set; }
    public int NoiseRefreshRate { get; set; }

    public bool EnableScanBeamBlur { get; set; }

    private string _scanBeamColorHex = "#5000FF00";

    [JsonIgnore]
    public Color ScanBeamColor
    {
        get => ColorUtils.HexToColor(_scanBeamColorHex);
        set => _scanBeamColorHex = ColorUtils.ColorToHex(value);
    }

    [JsonPropertyName("ScanBeamColor")]
    public string ScanBeamColorHex
    {
        get => _scanBeamColorHex;
        set => _scanBeamColorHex = ColorUtils.NormalizeColorHex(value);
    }

    public double ScanBeamHeight { get; set; }

    public bool EnableScanlineAnimation { get; set; }
    public int ScanlineAnimRefreshRate { get; set; }
    public double ScanlineAnimSpeed { get; set; }

    private string _scanlineColorHex = "#2300FF00";

    [JsonIgnore]
    public Color ScanlineColor
    {
        get => ColorUtils.HexToColor(_scanlineColorHex);
        set => _scanlineColorHex = ColorUtils.ColorToHex(value);
    }

    [JsonPropertyName("ScanlineColor")]
    public string ScanlineColorHex
    {
        get => _scanlineColorHex;
        set => _scanlineColorHex = ColorUtils.NormalizeColorHex(value);
    }

    public int ScanlineHeight { get; set; }
    public int ScanlineSpacing { get; set; }

    public bool UseCachedRendering { get; set; }

    
}