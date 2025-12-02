using System.Windows.Media;

namespace Pipboy.Wallpaper.Abstractions;


public interface IScanlineSettingsService
{
    void SetAnimation(bool enabled, int refreshRate, double speed);
    void SetStyle(Color color, int height, int spacing);
}
