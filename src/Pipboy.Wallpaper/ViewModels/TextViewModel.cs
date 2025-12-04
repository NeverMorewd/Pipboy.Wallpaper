using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipboy.Wallpaper.Framework;
using Pipboy.Wallpaper.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pipboy.Wallpaper.ViewModels;

public partial class TextViewModel : ReactiveObject
{
    private readonly IOptionsMonitor<TextOptionsModel> _textOptionsMonitor;
    private readonly DispatcherTimer _timer;
    private readonly ILogger _logger;
    public TextViewModel(IOptionsMonitor<TextOptionsModel> textOptionsMonitor, ILogger<TextViewModel> logger)
    {
        _textOptionsMonitor = textOptionsMonitor;
        _logger = logger;
        UpdateFromOptions(_textOptionsMonitor.CurrentValue);
        _textOptionsMonitor.OnChange(options =>
        {
            _logger.LogInformation("Text options changed, updating TextDataContext.");
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


    private void UpdateFromOptions(TextOptionsModel options)
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
