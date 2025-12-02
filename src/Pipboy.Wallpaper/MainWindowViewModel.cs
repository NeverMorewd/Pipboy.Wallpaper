using Pipboy.Wallpaper.Abstractions;
using Pipboy.Wallpaper.Utils;
using ReactiveUI.SourceGenerators;

namespace Pipboy.Wallpaper;

internal partial class MainWindowViewModel : ReactiveUI.ReactiveObject
{
    private readonly ICrtSettingsServiceFacade _crtSettingsServiceFacade;
    public MainWindowViewModel(ICrtSettingsServiceFacade crtSettingsServiceFacade)
    {
        _crtSettingsServiceFacade = crtSettingsServiceFacade;
        CrtDataContext = _crtSettingsServiceFacade.Data;
        TaskbarHeight = WindowsUtils.GetTaskbarHeight();
    }
    public CrtDataContext CrtDataContext { get;}
    [Reactive]
    private string _title = "Pipboy Wallpaper";
    [Reactive]
    private string _version = "v1.0.0";
    [Reactive]
    private string _content = "Welcome to Pipboy Wallpaper!";
    [Reactive]
    private double _taskbarHeight;
}
