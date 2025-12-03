using Pipboy.Wallpaper.Framework;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Pipboy.Wallpaper.Services;

public partial class TextSettingsService : ReactiveObject
{
    public TextSettingsService()
    {
#if DEBUG
        Version = AppDataContext.Current.Version;
#endif
    }

    [Reactive]
    private string _title = "Pipboy Wallpaper";
    [Reactive]
    private string _version = "";
    [Reactive]
    private string _content = "Welcome to Pipboy Wallpaper!";
}
