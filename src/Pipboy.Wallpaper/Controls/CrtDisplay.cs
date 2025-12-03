using Pipboy.Wallpaper.Utils;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Pipboy.Wallpaper.Controls;

/// <summary>
/// CPU-rendered CRT display with stable scanline animation and optional noise caching.
/// When UseCachedRendering=true, scanline animation is achieved by vertically shifting and wrapping the cached image.
/// Includes an optional FPS counter in the bottom-right corner.
/// </summary>
public class CrtDisplay : Decorator
{
    private readonly Random _random = new();
    private DispatcherTimer? _noiseTimer;

    // Animation state
    private double _scanlineAnimOffset;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private TimeSpan _lastRenderTime = TimeSpan.Zero;
    private bool _renderingSubscribed;

    // Rendering cache
    private Size _cachedSize = new(1, 1);
    private RenderTargetBitmap? _noiseCache;
    private RenderTargetBitmap? _scanlineCache;

    // FPS counter fields
    private int _frameCount = 0;
    private double _lastFpsUpdateTime = 0;
    private int _currentFps = 0;
    private static readonly double _taskBarHeight = WindowsUtils.GetTaskbarThickness();

    private readonly SolidColorBrush[] _noiseBrushCache = new SolidColorBrush[256];
    private double _lastCachedNoiseOpacity = -1;
    private FormattedText? _fpsTextCache;

    // Shorthand for property metadata flags
    private static readonly FrameworkPropertyMetadataOptions R = FrameworkPropertyMetadataOptions.AffectsRender;
    private static readonly FrameworkPropertyMetadataOptions M = FrameworkPropertyMetadataOptions.AffectsMeasure;

    // Dependency properties
    public static readonly DependencyProperty UseCachedRenderingProperty =
        DependencyProperty.Register(nameof(UseCachedRendering), typeof(bool), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(true, R));

    public static readonly DependencyProperty ScanlineColorProperty =
        DependencyProperty.Register(nameof(ScanlineColor), typeof(Color), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 255, 0), R, OnScanlinePropChanged));

    public static readonly DependencyProperty ScanlineSpacingProperty =
        DependencyProperty.Register(nameof(ScanlineSpacing), typeof(int), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(3, R | M, OnScanlinePropChanged));

    public static readonly DependencyProperty ScanlineHeightProperty =
        DependencyProperty.Register(nameof(ScanlineHeight), typeof(int), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(1, R | M, OnScanlinePropChanged));

    public static readonly DependencyProperty EnableNoiseProperty =
        DependencyProperty.Register(nameof(EnableNoise), typeof(bool), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(true, R));

    public static readonly DependencyProperty NoiseDensityProperty =
        DependencyProperty.Register(nameof(NoiseDensity), typeof(double), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(0.02, R));

    public static readonly DependencyProperty NoiseOpacityProperty =
        DependencyProperty.Register(nameof(NoiseOpacity), typeof(double), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(0.1, R));

    public static readonly DependencyProperty NoisePixelSizeProperty =
        DependencyProperty.Register(nameof(NoisePixelSize), typeof(int), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(2, R));

    public static readonly DependencyProperty NoiseRefreshRateProperty =
        DependencyProperty.Register(nameof(NoiseRefreshRate), typeof(int), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(50));

    public static readonly DependencyProperty EnableScanlineAnimationProperty =
        DependencyProperty.Register(nameof(EnableScanlineAnimation), typeof(bool), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(true, R));

    public static readonly DependencyProperty ScanlineAnimSpeedProperty =
        DependencyProperty.Register(nameof(ScanlineAnimSpeed), typeof(double), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(60.0, R));

    public static readonly DependencyProperty ScanBeamHeightProperty =
        DependencyProperty.Register(nameof(ScanBeamHeight), typeof(double), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(80.0, R));

    public static readonly DependencyProperty ScanBeamColorProperty =
        DependencyProperty.Register(nameof(ScanBeamColor), typeof(Color), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(Color.FromArgb(40, 0, 255, 0), R));

    public static readonly DependencyProperty EnableScanBeamBlurProperty =
        DependencyProperty.Register(nameof(EnableScanBeamBlur), typeof(bool), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(true, R));

    public static readonly DependencyProperty EnableFlickerProperty =
        DependencyProperty.Register(nameof(EnableFlicker), typeof(bool), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(false, R));

    public static readonly DependencyProperty FlickerIntensityProperty =
        DependencyProperty.Register(nameof(FlickerIntensity), typeof(double), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(0.05, R));

    public static readonly DependencyProperty ShowFpsProperty =
        DependencyProperty.Register(nameof(ShowFps), typeof(bool), typeof(CrtDisplay),
            new FrameworkPropertyMetadata(false, R));

    // CLR property wrappers
    public bool UseCachedRendering { get => (bool)GetValue(UseCachedRenderingProperty); set => SetValue(UseCachedRenderingProperty, value); }
    public Color ScanlineColor { get => (Color)GetValue(ScanlineColorProperty); set => SetValue(ScanlineColorProperty, value); }
    public int ScanlineSpacing { get => (int)GetValue(ScanlineSpacingProperty); set => SetValue(ScanlineSpacingProperty, value); }
    public int ScanlineHeight { get => (int)GetValue(ScanlineHeightProperty); set => SetValue(ScanlineHeightProperty, value); }
    public bool EnableNoise { get => (bool)GetValue(EnableNoiseProperty); set => SetValue(EnableNoiseProperty, value); }
    public double NoiseDensity { get => (double)GetValue(NoiseDensityProperty); set => SetValue(NoiseDensityProperty, value); }
    public double NoiseOpacity { get => (double)GetValue(NoiseOpacityProperty); set => SetValue(NoiseOpacityProperty, value); }
    public int NoisePixelSize { get => (int)GetValue(NoisePixelSizeProperty); set => SetValue(NoisePixelSizeProperty, value); }
    public int NoiseRefreshRate { get => (int)GetValue(NoiseRefreshRateProperty); set => SetValue(NoiseRefreshRateProperty, value); }
    public bool EnableScanlineAnimation { get => (bool)GetValue(EnableScanlineAnimationProperty); set => SetValue(EnableScanlineAnimationProperty, value); }
    public double ScanlineAnimSpeed { get => (double)GetValue(ScanlineAnimSpeedProperty); set => SetValue(ScanlineAnimSpeedProperty, value); }
    public double ScanBeamHeight { get => (double)GetValue(ScanBeamHeightProperty); set => SetValue(ScanBeamHeightProperty, value); }
    public Color ScanBeamColor { get => (Color)GetValue(ScanBeamColorProperty); set => SetValue(ScanBeamColorProperty, value); }
    public bool EnableScanBeamBlur { get => (bool)GetValue(EnableScanBeamBlurProperty); set => SetValue(EnableScanBeamBlurProperty, value); }
    public bool EnableFlicker { get => (bool)GetValue(EnableFlickerProperty); set => SetValue(EnableFlickerProperty, value); }
    public double FlickerIntensity { get => (double)GetValue(FlickerIntensityProperty); set => SetValue(FlickerIntensityProperty, value); }
    public bool ShowFps { get => (bool)GetValue(ShowFpsProperty); set => SetValue(ShowFpsProperty, value); }

    private static void OnScanlinePropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CrtDisplay c)
        {
            c._scanlineCache = null;
            c.InvalidateVisual();
        }
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);

        if (VisualParent != null)
        {
            Loaded += CrtDisplay_Loaded;
            Unloaded += CrtDisplay_Unloaded;
        }
        else
        {
            Loaded -= CrtDisplay_Loaded;
            Unloaded -= CrtDisplay_Unloaded;
            StopNoiseAnimation();
            UnsubscribeRendering();
        }
    }

    private void CrtDisplay_Loaded(object? sender, RoutedEventArgs e)
    {
        _scanlineAnimOffset = 0;
        StartNoiseAnimation();
        SubscribeRendering();
    }

    private void CrtDisplay_Unloaded(object? sender, RoutedEventArgs e)
    {
        StopNoiseAnimation();
        UnsubscribeRendering();
    }

    private void StartNoiseAnimation()
    {
        if (_noiseTimer != null || !EnableNoise) return;

        _noiseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(NoiseRefreshRate) };
        _noiseTimer.Tick += (_, _) =>
        {
            if (UseCachedRendering) RegenerateNoiseCache();
            InvalidateVisual();
        };
        _noiseTimer.Start();
    }

    private void StopNoiseAnimation()
    {
        _noiseTimer?.Stop();
        _noiseTimer = null;
    }

    private void SubscribeRendering()
    {
        if (_renderingSubscribed) return;
        CompositionTarget.Rendering += CompositionTarget_Rendering;
        _lastRenderTime = _stopwatch.Elapsed;
        _renderingSubscribed = true;
    }

    private void UnsubscribeRendering()
    {
        if (!_renderingSubscribed) return;
        CompositionTarget.Rendering -= CompositionTarget_Rendering;
        _renderingSubscribed = false;
    }

    private void CompositionTarget_Rendering(object? sender, EventArgs e)
    {
        var now = _stopwatch.Elapsed;
        var totalSeconds = now.TotalSeconds;
        var dt = (now - _lastRenderTime).TotalSeconds;
        _lastRenderTime = now;

        // Update FPS counter
        _frameCount++;
        if (totalSeconds - _lastFpsUpdateTime >= 1.0)
        {
            int newFps = _frameCount;
            if (_currentFps != newFps)
            {
                _currentFps = newFps;
                _fpsTextCache = null; // Rebuild text on change
            }
            _frameCount = 0;
            _lastFpsUpdateTime = totalSeconds;
        }

        // Update scanline animation offset
        if (EnableScanlineAnimation)
        {
            _scanlineAnimOffset += ScanlineAnimSpeed * dt;
            if (ScanlineSpacing > 0)
                _scanlineAnimOffset = Mod(_scanlineAnimOffset, ScanlineSpacing);
        }

        // Redraw if animation or FPS display is active
        if (EnableScanlineAnimation || ShowFps)
        {
            InvalidateVisual();
        }
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        var rect = new Rect(0, 0, ActualWidth, ActualHeight);
        if (rect.Width <= 0 || rect.Height <= 0) return;

        if (UseCachedRendering)
            RenderCached(dc, rect);
        else
            RenderDirect(dc, rect);

        if (ShowFps)
        {
            DrawFps(dc, rect);
        }
    }

    private void DrawFps(DrawingContext dc, Rect rect)
    {
        if (_fpsTextCache == null)
        {
            var typeface = new Typeface(new FontFamily("Consolas, Courier New, Monospace"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
            var brush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brush.Freeze();

            _fpsTextCache = new FormattedText(
                $"FPS: {_currentFps}",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                14,
                brush,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);
        }

        var x = rect.Width - _fpsTextCache.Width - 5;
        var y = rect.Height - _fpsTextCache.Height - 5 - _taskBarHeight;

        var bgBrush = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
        bgBrush.Freeze();

        dc.DrawRectangle(bgBrush, null, new Rect(x - 2, y - 2, _fpsTextCache.Width + 4, _fpsTextCache.Height + 4));
        dc.DrawText(_fpsTextCache, new Point(x, y));
    }

    private void RenderCached(DrawingContext dc, Rect rect)
    {
        bool sizeChanged = _cachedSize.Width != rect.Width || _cachedSize.Height != rect.Height;

        if (sizeChanged || _scanlineCache == null || _noiseCache == null)
        {
            _cachedSize = rect.Size;
            RegenerateScanlineCache();
            RegenerateNoiseCache();
        }

        if (_scanlineCache != null)
        {
            double offset = 0;
            if (EnableScanlineAnimation && ScanlineSpacing > 0)
            {
                offset = Mod(_scanlineAnimOffset, ScanlineSpacing);
            }

            if (Math.Abs(offset) > 0.001)
            {
                // Main part
                dc.PushTransform(new TranslateTransform(0, -offset));
                dc.DrawImage(_scanlineCache, rect);
                dc.Pop();

                // Wrapped continuation
                dc.PushTransform(new TranslateTransform(0, rect.Height - offset));
                dc.DrawImage(_scanlineCache, rect);
                dc.Pop();
            }
            else
            {
                dc.DrawImage(_scanlineCache, rect);
            }
        }

        if (EnableNoise && _noiseCache != null)
            dc.DrawImage(_noiseCache, rect);

        if (EnableScanlineAnimation)
            DrawScanBeam(dc, rect);

        ApplyFlicker(dc);
    }

    private void RenderDirect(DrawingContext dc, Rect rect)
    {
        if (EnableNoise)
            DrawNoise(dc, rect);

        if (EnableScanlineAnimation && ScanlineSpacing > 0)
        {
            double offset = Mod(_scanlineAnimOffset, ScanlineSpacing);
            DrawScanlines(dc, new Rect(0, -offset, rect.Width, rect.Height + offset));
        }
        else
        {
            DrawScanlines(dc, rect);
        }

        if (EnableScanlineAnimation)
            DrawScanBeam(dc, rect);

        ApplyFlicker(dc);
    }

    private void RegenerateScanlineCache()
    {
        int w = Math.Max(1, (int)Math.Ceiling(_cachedSize.Width));
        int h = Math.Max(1, (int)Math.Ceiling(_cachedSize.Height));

        _scanlineCache = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);

        var dv = new DrawingVisual();
        using (var dc = dv.RenderOpen())
        {
            var brush = new SolidColorBrush(ScanlineColor);
            brush.Freeze();
            var pen = new Pen(brush, Math.Max(1, ScanlineHeight));
            pen.Freeze();

            int spacing = Math.Max(1, ScanlineSpacing);

            for (double y = 0; y < h; y += spacing)
            {
                dc.DrawLine(pen, new Point(0, y), new Point(w, y));
            }
        }

        _scanlineCache.Render(dv);
        if (_scanlineCache.CanFreeze) _scanlineCache.Freeze();
    }

    private void RegenerateNoiseCache()
    {
        int w = Math.Max(1, (int)_cachedSize.Width);
        int h = Math.Max(1, (int)_cachedSize.Height);
        _noiseCache = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
        var dv = new DrawingVisual();
        using (var dc = dv.RenderOpen())
            DrawNoise(dc, new Rect(0, 0, w, h));
        _noiseCache.Render(dv);
        if (_noiseCache.CanFreeze) _noiseCache.Freeze();
    }

    private void DrawNoise(DrawingContext dc, Rect rect)
    {
        int pixel = Math.Max(1, NoisePixelSize);

        if (Math.Abs(_lastCachedNoiseOpacity - NoiseOpacity) > 0.001)
        {
            _lastCachedNoiseOpacity = NoiseOpacity;
            byte alpha = (byte)Math.Clamp((int)(NoiseOpacity * 255), 0, 255);

            for (int i = 0; i < 256; i++)
            {
                byte b = (byte)i;
                var brush = new SolidColorBrush(Color.FromArgb(alpha, b, b, b));
                brush.Freeze();
                _noiseBrushCache[i] = brush;
            }
        }

        for (int x = 0; x < rect.Width; x += pixel)
        {
            for (int y = 0; y < rect.Height; y += pixel)
            {
                if (_random.NextDouble() > NoiseDensity) continue;

                int colorIndex = _random.Next(256);
                dc.DrawRectangle(_noiseBrushCache[colorIndex], null, new Rect(x, y, pixel, pixel));
            }
        }
    }

    private void DrawScanlines(DrawingContext dc, Rect rect)
    {
        var brush = new SolidColorBrush(ScanlineColor);
        brush.Freeze();
        var pen = new Pen(brush, Math.Max(1, ScanlineHeight));
        pen.Freeze();

        int spacing = Math.Max(1, ScanlineSpacing);

        for (double y = rect.Y; y < rect.Y + rect.Height; y += spacing)
        {
            dc.DrawLine(pen, new Point(0, y), new Point(rect.Width, y));
        }
    }

    private void DrawScanBeam(DrawingContext dc, Rect rect)
    {
        double seconds = _stopwatch.Elapsed.TotalSeconds;
        double beamY = Mod(seconds * ScanlineAnimSpeed, ActualHeight + ScanBeamHeight) - ScanBeamHeight;

        if (beamY < -ScanBeamHeight || beamY > rect.Height) return;

        if (EnableScanBeamBlur)
        {
            var brush = new LinearGradientBrush
            {
                MappingMode = BrushMappingMode.Absolute,
                StartPoint = new Point(0, beamY),
                EndPoint = new Point(0, beamY + ScanBeamHeight)
            };
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, ScanBeamColor.R, ScanBeamColor.G, ScanBeamColor.B), 0.0));
            brush.GradientStops.Add(new GradientStop(ScanBeamColor, 0.3));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, ScanBeamColor.R, ScanBeamColor.G, ScanBeamColor.B), 1.0));
            brush.Freeze();
            dc.DrawRectangle(brush, null, new Rect(0, beamY, rect.Width, ScanBeamHeight));
        }
        else
        {
            var brush = new SolidColorBrush(ScanBeamColor);
            brush.Freeze();
            dc.DrawRectangle(brush, null, new Rect(0, beamY, rect.Width, ScanBeamHeight));
        }
    }

    private void ApplyFlicker(DrawingContext dc)
    {
        if (!EnableFlicker) return;
        double factor = 1.0 + ((_random.NextDouble() - 0.5) * FlickerIntensity);
        factor = Math.Clamp(factor, 0.0, 2.0);
        dc.PushOpacity(factor);
        dc.Pop();
    }

    private static double Mod(double x, double m)
    {
        if (m == 0) return 0;
        var r = x % m;
        if (r < 0) r += m;
        return r;
    }
}