using Pipboy.Wallpaper.Abstractions;
using Pipboy.Wallpaper.Models;

namespace Pipboy.Wallpaper.Services;

public class NoiseSettingsService : INoiseSettingsService
{
    private readonly CrtDataContext _data;

    public NoiseSettingsService(CrtDataContext data) => _data = data;

    public void SetEnabled(bool enabled) =>
        _data.EnableNoise = enabled;

    public void SetNoise(double density, double opacity, int pixelSize, int refreshRate)
    {
        _data.NoiseDensity = density;
        _data.NoiseOpacity = opacity;
        _data.NoisePixelSize = pixelSize;
        _data.NoiseRefreshRate = refreshRate;
    }
}
