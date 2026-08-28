using System.Windows;
using System.Windows.Media;

namespace CuteGreenCalculator.Styles;

/// <summary>
/// Attached property holding the "pressed" art for a pixel button. The button's
/// normal art is set via the standard <see cref="System.Windows.Controls.Control.Tag"/>
/// property (bound to Image.Source in the shared template); this attached property
/// supplies the second image the IsPressed trigger swaps to.
/// </summary>
public static class PixelButton
{
    public static readonly DependencyProperty PressedSourceProperty =
        DependencyProperty.RegisterAttached(
            "PressedSource",
            typeof(ImageSource),
            typeof(PixelButton));

    public static void SetPressedSource(DependencyObject element, ImageSource value) =>
        element.SetValue(PressedSourceProperty, value);

    public static ImageSource GetPressedSource(DependencyObject element) =>
        (ImageSource)element.GetValue(PressedSourceProperty);
}
