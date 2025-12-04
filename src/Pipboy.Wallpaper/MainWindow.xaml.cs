using Microsoft.Extensions.Logging;
using ReactiveUI;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;

namespace Pipboy.Wallpaper;

public partial class MainWindow : Window
{
    private readonly ILogger _logger;
    public MainWindow(ILogger<MainWindow> logger)
    {
        InitializeComponent();
        _logger = logger;
//#if DEBUG
//        WindowState = WindowState.Maximized;
//        WindowStyle = WindowStyle.None;
//#endif
        //MainBorder.Effect = new FisheyeEffect
        //{
        //    Radius = 0.25,
        //    Strength = 2.0,
        //    Center = new Point(0.5, 0.5),
        //    Aspect = 1.0
        //};
        //CreateCursorStream(50, 10)
        //    .ObserveOn(RxApp.MainThreadScheduler)
        //    .Subscribe(p=> 
        //    {
        //        OnMouseMove(MainBorder, p);
        //    });
    }
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        if (MainBorder.Effect is FisheyeEffect effect && sizeInfo.NewSize.Height > 0)
        {
            effect.Aspect = sizeInfo.NewSize.Width / sizeInfo.NewSize.Height;
        }
    }
    private void OnMouseMove(FrameworkElement element, Point point)
    {
        if (element.Effect is not FisheyeEffect effect) return;

        double u = point.X / element.ActualWidth;
        double v = point.Y / element.ActualHeight;

        double aspect = element.ActualWidth / element.ActualHeight;

        effect.Center = new Point(u, v);
        effect.Aspect = aspect;

        effect.Radius = 0.2;
        effect.Strength = 2.5;
    }
    private IObservable<Point> CreateCursorStream(int interval, int throttle)
    {
        return Observable.Interval(TimeSpan.FromMilliseconds(interval))
            .ObserveOn(TaskPoolScheduler.Default)
            .Select(_ => System.Windows.Forms.Cursor.Position)
            .DistinctUntilChanged(args => (args.X, args.Y))
            .Throttle(TimeSpan.FromMilliseconds(throttle))
            .Select(p => new Point(p.X, p.Y))
            .Publish()
            .RefCount();
    }
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        //var source = (HwndSource)PresentationSource.FromVisual(this);
        //source.AddHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_CLOSE = 0x0010;
        const int WM_DESTROY = 0x0002;
        const int WM_NCDESTROY = 0x0082;
        const int WM_QUIT = 0x0012;

        switch (msg)
        {
            case WM_CLOSE:
            case WM_DESTROY:
            case WM_NCDESTROY:
            case WM_QUIT:
                _logger?.LogWarning("WndProc intercepted message: {msg} (0x{msgX:X4})", msg, msg);
                return IntPtr.Zero;
        }

        return IntPtr.Zero;
    }
}