using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pipboy.Wallpaper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int _frameCount = 0;
    private readonly DispatcherTimer _fpsTimer;
    public MainWindow()
    {
        InitializeComponent();
        CompositionTarget.Rendering += OnRendering;
        _fpsTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), 
            DispatcherPriority.Normal, 
            OnFpsTimerTick, 
            Dispatcher);
    }
    private void OnRendering(object? sender, EventArgs e)
    {
        _frameCount++;
    }

    private void OnFpsTimerTick(object? sender, EventArgs e)
    {
        var fps = _frameCount;
        _frameCount = 0;
        //Title = $"FPS: {fps}";
    }
}