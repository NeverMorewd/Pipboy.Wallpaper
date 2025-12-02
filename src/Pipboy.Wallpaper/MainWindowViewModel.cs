using Pipboy.Wallpaper.Abstractions;
using Pipboy.Wallpaper.Framework;
using Pipboy.Wallpaper.Utils;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Pipboy.Wallpaper;

internal partial class MainWindowViewModel : ReactiveObject
{
    private readonly ICrtSettingsServiceFacade _crtSettingsServiceFacade;
    public MainWindowViewModel(ICrtSettingsServiceFacade crtSettingsServiceFacade)
    {
        _crtSettingsServiceFacade = crtSettingsServiceFacade;
        CrtDataContext = _crtSettingsServiceFacade.Data;
        TaskbarHeight = WindowsUtils.GetTaskbarHeight();

#if DEBUG
        Version = AppDataContext.Current.Version;
#endif
    }
    public CrtDataContext CrtDataContext { get;}
    [Reactive]
    private string _title = "Pipboy Wallpaper";
    [Reactive]
    private string _version = "";
    [Reactive]
    private string _content = "Welcome to Pipboy Wallpaper!";
    [Reactive]
    private double _taskbarHeight;
}
