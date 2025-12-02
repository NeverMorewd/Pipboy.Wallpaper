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
    public Color ScanBeamColor { get; set; }
    public double ScanBeamHeight { get; set; }

    public bool EnableScanlineAnimation { get; set; }
    public int ScanlineAnimRefreshRate { get; set; }
    public double ScanlineAnimSpeed { get; set; }

    public Color ScanlineColor { get; set; }
    public int ScanlineHeight { get; set; }
    public int ScanlineSpacing { get; set; }

    public bool UseCachedRendering { get; set; }
}
