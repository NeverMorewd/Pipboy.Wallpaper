namespace Pipboy.Wallpaper.Abstractions;

public interface INoiseSettingsService
{
    void SetEnabled(bool enabled);
    void SetNoise(double density, double opacity, int pixelSize, int refreshRate);
}
