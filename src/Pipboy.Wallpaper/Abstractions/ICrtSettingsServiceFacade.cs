using Pipboy.Wallpaper.Models;

namespace Pipboy.Wallpaper.Abstractions;

public interface ICrtSettingsServiceFacade
{
    CrtDataContext Data { get; }
    TextDataContext TextData { get; }

    INoiseSettingsService Noise { get; }
    IScanBeamSettingsService ScanBeam { get; }
    IScanlineSettingsService Scanline { get; }

    void ApplyConfig(CrtOptionsDto config);
    CrtOptionsDto ExportConfig();

    Task SaveToJsonAsync(string path);
    Task LoadFromJsonAsync(string path);
}
