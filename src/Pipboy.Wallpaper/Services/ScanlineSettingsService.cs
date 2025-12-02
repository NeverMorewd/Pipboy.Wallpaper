using Pipboy.Wallpaper.Abstractions;
using System.Windows.Media;

namespace Pipboy.Wallpaper.Services;

public class ScanlineSettingsService : IScanlineSettingsService
{
    private readonly CrtDataContext _data;

    public ScanlineSettingsService(CrtDataContext data) => _data = data;

    public void SetAnimation(bool enabled, int refreshRate, double speed)
    {
        _data.EnableScanlineAnimation = enabled;
        _data.ScanlineAnimRefreshRate = refreshRate;
        _data.ScanlineAnimSpeed = speed;
    }

    public void SetStyle(Color color, int height, int spacing)
    {
        _data.ScanlineColor = color;
        _data.ScanlineHeight = height;
        _data.ScanlineSpacing = spacing;
    }
}
