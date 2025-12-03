using System.Windows;
using System.Windows.Interop;
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
#if DEBUG
        CompositionTarget.Rendering += OnRendering;
        _fpsTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), 
            DispatcherPriority.Normal, 
            OnFpsTimerTick, 
            Dispatcher);
#endif
    }
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var source = (HwndSource)PresentationSource.FromVisual(this);
        source.AddHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_DESTROY = 0x0002;
        const int WM_NCDESTROY = 0x0082;

        if (msg == WM_DESTROY || msg == WM_NCDESTROY)
        {
            handled = true;
            return IntPtr.Zero;
        }

        return IntPtr.Zero;
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