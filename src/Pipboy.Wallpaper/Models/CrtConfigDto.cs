using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Pipboy.Wallpaper.Models;

public class CrtConfigDto
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
        get => HexToColor(_scanBeamColorHex);
        set => _scanBeamColorHex = ColorToHex(value);
    }

    [JsonPropertyName("ScanBeamColor")]
    public string ScanBeamColorHex
    {
        get => _scanBeamColorHex;
        set => _scanBeamColorHex = NormalizeColorHex(value);
    }

    public double ScanBeamHeight { get; set; }

    public bool EnableScanlineAnimation { get; set; }
    public int ScanlineAnimRefreshRate { get; set; }
    public double ScanlineAnimSpeed { get; set; }

    private string _scanlineColorHex = "#2300FF00";

    [JsonIgnore]
    public Color ScanlineColor
    {
        get => HexToColor(_scanlineColorHex);
        set => _scanlineColorHex = ColorToHex(value);
    }

    [JsonPropertyName("ScanlineColor")]
    public string ScanlineColorHex
    {
        get => _scanlineColorHex;
        set => _scanlineColorHex = NormalizeColorHex(value);
    }

    public int ScanlineHeight { get; set; }
    public int ScanlineSpacing { get; set; }

    public bool UseCachedRendering { get; set; }

    private static string ColorToHex(Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    private static string NormalizeColorHex(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Color hex cannot be null or empty.");

        var hex = input.Trim();
        if (hex.StartsWith("#"))
            hex = hex.Substring(1);

        if (hex.Length != 6 && hex.Length != 8)
            throw new ArgumentException("Color hex must be 6 (RRGGBB) or 8 (AARRGGBB) hex digits.");

        if (hex.Length == 6)
            hex = "FF" + hex;

        _ = Convert.ToUInt32(hex, 16);

        return "#" + hex.ToUpperInvariant();
    }

    private static Color HexToColor(string hexWithHash)
    {
        if (string.IsNullOrWhiteSpace(hexWithHash))
            throw new ArgumentException("Color hex string cannot be null or empty.");

        var hex = hexWithHash.TrimStart('#');

        if (hex.Length == 6)
        {
            var r = Convert.ToByte(hex.Substring(0, 2), 16);
            var g = Convert.ToByte(hex.Substring(2, 2), 16);
            var b = Convert.ToByte(hex.Substring(4, 2), 16);
            return Color.FromArgb(255, r, g, b);
        }
        else if (hex.Length == 8)
        {
            var a = Convert.ToByte(hex.Substring(0, 2), 16);
            var r = Convert.ToByte(hex.Substring(2, 2), 16);
            var g = Convert.ToByte(hex.Substring(4, 2), 16);
            var b = Convert.ToByte(hex.Substring(6, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }
        else
        {
            throw new ArgumentException($"Invalid color hex format: {hexWithHash}. Expected #RRGGBB or #AARRGGBB.");
        }
    }
}