using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.IO;

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

    public string AppTempDirectory =>  Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    AppName);

    public string Version { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
}
