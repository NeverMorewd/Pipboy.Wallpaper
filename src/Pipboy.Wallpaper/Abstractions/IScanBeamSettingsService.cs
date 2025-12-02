using System.Windows.Media;

namespace Pipboy.Wallpaper.Abstractions;

public interface IScanBeamSettingsService
{
    void Set(bool enabled, Color color, double height);
}
