using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Pipboy.Wallpaper.Framework;

internal sealed partial class AppDataContext : ReactiveObject
{
    private static readonly Lazy<AppDataContext> _instance =
        new(() => new AppDataContext());

    public static AppDataContext Current => _instance.Value;

    private AppDataContext()
    {

    }

    [Reactive]
    private string _appName = "Pipboy Wallpaper";
}
