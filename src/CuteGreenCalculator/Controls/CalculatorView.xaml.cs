using System.Windows.Controls;

namespace CuteGreenCalculator.Controls;

/// <summary>
/// Renders the calculator face: background art, screen, and button grid.
/// Deliberately has no knowledge of the hosting window's chrome (title bar,
/// resize mode, etc.) so a future custom borderless frame can host this
/// control unchanged.
/// </summary>
public partial class CalculatorView : UserControl
{
    public CalculatorView()
    {
        InitializeComponent();
    }
}
