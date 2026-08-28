using System.Windows;
using System.Windows.Media;

namespace CuteGreenCalculator.Styles;

/// <summary>
/// Attached properties holding the alternate art for a pixel button/toggle
/// button. The normal art is set via the standard
/// <see cref="System.Windows.Controls.Control.Tag"/> property (bound to
/// Image.Source in the shared template); these attached properties supply
/// the second image a trigger swaps to.
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

    /// <summary>The art shown while a <see cref="System.Windows.Controls.Primitives.ToggleButton"/> is checked.</summary>
    public static readonly DependencyProperty CheckedSourceProperty =
        DependencyProperty.RegisterAttached(
            "CheckedSource",
            typeof(ImageSource),
            typeof(PixelButton));

    public static void SetCheckedSource(DependencyObject element, ImageSource value) =>
        element.SetValue(CheckedSourceProperty, value);

    public static ImageSource GetCheckedSource(DependencyObject element) =>
        (ImageSource)element.GetValue(CheckedSourceProperty);
}
