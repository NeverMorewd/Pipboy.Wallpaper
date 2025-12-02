using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Windows.Media;

namespace Pipboy.Wallpaper;

public partial class CrtDataContext : ReactiveObject
{
    [Reactive] 
    private bool _enableNoise = true;
    [Reactive] 
    private bool _enableScanBeamBlur = true;
    [Reactive] 
    private bool _enableScanlineAnimation = true;

    [Reactive] 
    private double _noiseDensity = 0.02;
    [Reactive] 
    private double _noiseOpacity = 0.3;
    [Reactive] 
    private int _noisePixelSize = 3;
    [Reactive] 
    private int _noiseRefreshRate = 50;

    [Reactive] 
    private Color _scanBeamColor = Color.FromArgb(80, 0, 255, 0);
    [Reactive] 
    private double _scanBeamHeight = 100;

    [Reactive]
    private int _scanlineAnimRefreshRate = 16;
    [Reactive] 
    private double _scanlineAnimSpeed = 100;

    [Reactive] 
    private Color _scanlineColor = Color.FromArgb(35, 0, 255, 0);
    [Reactive] 
    private int _scanlineHeight = 5;
    [Reactive] 
    private int _scanlineSpacing = 15;

    [Reactive] 
    private bool _useCachedRendering = true;
}
