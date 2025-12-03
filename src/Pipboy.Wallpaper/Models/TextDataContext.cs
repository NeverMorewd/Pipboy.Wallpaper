using Microsoft.Extensions.Options;
using Pipboy.Wallpaper.Framework;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pipboy.Wallpaper.Models;

public partial class TextDataContext : ReactiveObject
{
    private readonly IOptionsMonitor<TextOptionsDto> _textOptionsMonitor;
    private readonly DispatcherTimer _timer;
    public TextDataContext(IOptionsMonitor<TextOptionsDto> textOptionsMonitor)
    {
        _textOptionsMonitor = textOptionsMonitor;
        UpdateFromOptions(_textOptionsMonitor.CurrentValue);
        _textOptionsMonitor.OnChange(options =>
        {
            UpdateFromOptions(options);
        });
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (s, e) => CurrentDateTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _timer.Start();

        CurrentDateTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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
    [Reactive]
    private bool _showFps = true;
    [Reactive]
    private bool _showDatetime = false;
    [Reactive]
    private FontFamily _fontFamily = new("Courier New");
    [Reactive]
    private SolidColorBrush _textForeground = new(System.Windows.Media.Colors.Lime);

    [Reactive]
    private string _currentDateTimeString;
    [Reactive]
    private int _contentFontSize = 30;


    private void UpdateFromOptions(TextOptionsDto options)
    {
        Title = options.Title;
        Content = options.Content;
        ShowDatetime = options.ShowDatetime;
        ShowFps = options.ShowFps;
        FontFamily = new FontFamily(options.FontFamily);
        TextForeground = new SolidColorBrush(options.ForegroundColor);
        ContentFontSize = options.ContentFontSize;
    }
}
