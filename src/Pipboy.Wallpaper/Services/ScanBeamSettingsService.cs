using Pipboy.Wallpaper.Abstractions;
using System.Windows.Media;

namespace Pipboy.Wallpaper.Services;

public class ScanBeamSettingsService : IScanBeamSettingsService
{
    private readonly CrtDataContext _data;

    public ScanBeamSettingsService(CrtDataContext data) => _data = data;

    public void Set(bool enabled, Color color, double height)
    {
        _data.EnableScanBeamBlur = enabled;
        _data.ScanBeamColor = color;
        _data.ScanBeamHeight = height;
    }
}
