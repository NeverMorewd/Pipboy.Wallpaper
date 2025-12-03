using Pipboy.Wallpaper.Abstractions;
using Pipboy.Wallpaper.Models;
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
        TextDataContext = _crtSettingsServiceFacade.TextData;
        TaskbarHeight = WindowsUtils.GetTaskbarThickness();
    }
    public CrtDataContext CrtDataContext { get;}
    public TextDataContext TextDataContext { get; }

    [Reactive]
    private double _taskbarHeight;
}
