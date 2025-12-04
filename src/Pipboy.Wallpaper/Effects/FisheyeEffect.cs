using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

public class FisheyeEffect : ShaderEffect
{
    private static readonly PixelShader _pixelShader = new()
    {
        UriSource = new Uri("pack://application:,,,/Pipboy.Wallpaper;component/Effects/Shaders/magnify.ps")
    };

    public FisheyeEffect()
    {
        PixelShader = _pixelShader;
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(CenterProperty);
        UpdateShaderValue(RadiusProperty);
        UpdateShaderValue(StrengthProperty);
        UpdateShaderValue(AspectProperty);
    }

    public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(FisheyeEffect), 0);
    public Brush Input
    {
        get { return (Brush)GetValue(InputProperty); }
        set { SetValue(InputProperty, value); }
    }

    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(FisheyeEffect), new UIPropertyMetadata(new Point(0.5, 0.5), PixelShaderConstantCallback(0)));
    public Point Center 
    { 
        get { return (Point)GetValue(CenterProperty); } 
        set { SetValue(CenterProperty, value); } 
    }

    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(FisheyeEffect), new UIPropertyMetadata(0.25, PixelShaderConstantCallback(1)));
    public double Radius 
    { 
        get { return (double)GetValue(RadiusProperty); } 
        set { SetValue(RadiusProperty, value); } 
    }

    public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register("Strength", typeof(double), typeof(FisheyeEffect), new UIPropertyMetadata(2.0, PixelShaderConstantCallback(2)));
    public double Strength 
    { 
        get { return (double)GetValue(StrengthProperty); } 
        set { SetValue(StrengthProperty, value); }
    }

    public static readonly DependencyProperty AspectProperty = DependencyProperty.Register("Aspect", typeof(double), typeof(FisheyeEffect), new UIPropertyMetadata(1.0, PixelShaderConstantCallback(3)));
    public double Aspect 
    { 
        get { return (double)GetValue(AspectProperty); } 
        set { SetValue(AspectProperty, value); } 
    }
}