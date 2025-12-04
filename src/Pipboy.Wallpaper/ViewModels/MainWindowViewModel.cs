using Pipboy.Wallpaper.Abstractions;
using Pipboy.Wallpaper.Utils;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Pipboy.Wallpaper.ViewModels;

internal partial class MainWindowViewModel : ReactiveObject
{
    private readonly IDataContextFacade _dataContextFacade;
    public MainWindowViewModel(IDataContextFacade dataContextFacade)
    {
        _dataContextFacade = dataContextFacade;
        EffectDataContext = _dataContextFacade.EffectData;
        TextDataContext = _dataContextFacade.TextData;
        TaskbarHeight = WindowsUtils.GetTaskbarThickness();
    }
    public EffectViewModel EffectDataContext { get;}
    public TextViewModel TextDataContext { get; }

    [Reactive]
    private double _taskbarHeight;
}
