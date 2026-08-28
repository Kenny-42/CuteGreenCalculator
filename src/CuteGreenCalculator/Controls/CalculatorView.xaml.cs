using System.Windows;
using System.Windows.Controls;

namespace CuteGreenCalculator.Controls;

/// <summary>
/// Renders the calculator face: background art, screen, and button grid.
/// Deliberately has no knowledge of the hosting window's chrome (title bar,
/// resize mode, etc.) so a future custom borderless frame can host this
/// control unchanged.
///
/// Owns one <see cref="CalculatorEngine"/> instance and wires every
/// digit/operator/function/equals button to it. The 45/90/180 speed-dial
/// buttons are wired separately (see add-speed-dial-buttons).
/// </summary>
public partial class CalculatorView : UserControl
{
    private readonly CalculatorEngine _engine = new();

    public CalculatorView()
    {
        InitializeComponent();
        WireButtons();
        RefreshDisplay();
    }

    private void WireButtons()
    {
        Btn0.Click += (_, _) => Handle(() => _engine.InputDigit('0'));
        Btn1.Click += (_, _) => Handle(() => _engine.InputDigit('1'));
        Btn2.Click += (_, _) => Handle(() => _engine.InputDigit('2'));
        Btn3.Click += (_, _) => Handle(() => _engine.InputDigit('3'));
        Btn4.Click += (_, _) => Handle(() => _engine.InputDigit('4'));
        Btn5.Click += (_, _) => Handle(() => _engine.InputDigit('5'));
        Btn6.Click += (_, _) => Handle(() => _engine.InputDigit('6'));
        Btn7.Click += (_, _) => Handle(() => _engine.InputDigit('7'));
        Btn8.Click += (_, _) => Handle(() => _engine.InputDigit('8'));
        Btn9.Click += (_, _) => Handle(() => _engine.InputDigit('9'));

        BtnDecimal.Click += (_, _) => Handle(_engine.InputDecimalPoint);

        BtnAdd.Click += (_, _) => Handle(() => _engine.InputOperator('+'));
        BtnSubtract.Click += (_, _) => Handle(() => _engine.InputOperator('-'));
        BtnMultiply.Click += (_, _) => Handle(() => _engine.InputOperator('*'));
        BtnDivide.Click += (_, _) => Handle(() => _engine.InputOperator('/'));

        BtnEquals.Click += (_, _) => Handle(_engine.Equals);
        BtnClear.Click += (_, _) => Handle(_engine.Clear);
        BtnClearEntry.Click += (_, _) => Handle(_engine.ClearEntry);
        BtnSign.Click += (_, _) => Handle(_engine.ToggleSign);
        BtnSqrt.Click += (_, _) => Handle(_engine.SquareRoot);
    }

    /// <summary>Runs an engine action, then refreshes the display.</summary>
    private void Handle(Action action)
    {
        action();
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        DisplayText.Text = _engine.Display;
    }
}
