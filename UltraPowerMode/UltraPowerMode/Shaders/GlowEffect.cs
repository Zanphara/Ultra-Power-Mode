using System.Windows.Media.Effects;
using System.Windows;
using System;
using System.Windows.Media;

public class GlowEffect : ShaderEffect
{
    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(GlowEffect), 0);
    public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register("GlowColor", typeof(Color), typeof(GlowEffect), new UIPropertyMetadata(Color.FromArgb(255, 255, 255, 255), PixelShaderConstantCallback(0)));
    public static readonly DependencyProperty GlowSizeProperty = DependencyProperty.Register("GlowSize", typeof(double), typeof(GlowEffect), new UIPropertyMetadata(10.0, PixelShaderConstantCallback(1)));

    public GlowEffect()
    {
        PixelShader = new PixelShader { UriSource = new Uri("pack://application:,,,/YourAssembly;component/GlowEffect.ps") };
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(GlowColorProperty);
        UpdateShaderValue(GlowSizeProperty);
    }

    public Brush Input
    {
        get { return (Brush)GetValue(InputProperty); }
        set { SetValue(InputProperty, value); }
    }

    public Color GlowColor
    {
        get { return (Color)GetValue(GlowColorProperty); }
        set { SetValue(GlowColorProperty, value); }
    }

    public double GlowSize
    {
        get { return (double)GetValue(GlowSizeProperty); }
        set { SetValue(GlowSizeProperty, value); }
    }
}