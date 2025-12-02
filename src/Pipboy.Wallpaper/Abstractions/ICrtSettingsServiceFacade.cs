using Pipboy.Wallpaper.Models;

namespace Pipboy.Wallpaper.Abstractions;

public interface ICrtSettingsServiceFacade
{
    CrtDataContext Data { get; }

    INoiseSettingsService Noise { get; }
    IScanBeamSettingsService ScanBeam { get; }
    IScanlineSettingsService Scanline { get; }

    void ApplyConfig(CrtConfigDto config);
    CrtConfigDto ExportConfig();

    void SaveToJson(string path);
    void LoadFromJson(string path);
}
