using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CuteGreenCalculator.Controls;

/// <summary>
/// The calculator face's visual states. Focus tracking (issue #18) is the
/// first driver of this; more states can be added here later (e.g. an
/// idle/blink animation, an error face) by extending the enum and
/// <see cref="FaceStateAssets"/> only - <see cref="CalculatorView.SetFaceState"/>
/// and its call sites don't need to change.
/// </summary>
public enum FaceState
{
    Awake,
    Asleep,
}

/// <summary>
/// Renders the calculator face: background art, screen, and button grid.
/// Deliberately has no knowledge of the hosting window's chrome (title bar,
/// resize mode, etc.) so a future custom borderless frame can host this
/// control unchanged. The always-on-top toggle and window focus are
/// window-level concerns, so this view only raises
/// <see cref="AlwaysOnTopChanged"/> / exposes <see cref="SetFocused"/> and
/// leaves touching <c>Window</c> itself to whoever hosts it.
///
/// Owns one <see cref="CalculatorEngine"/> instance and wires every
/// digit/operator/function/equals button to it, including the 45/90/180
/// speed-dial shortcuts, which feed their digits through the same
/// digit-entry path as typing them individually. Keyboard input and
/// clipboard copy/paste drive the same engine paths as the buttons.
/// </summary>
public partial class CalculatorView : UserControl
{
    private static readonly Dictionary<FaceState, BitmapImage> FaceStateAssets = new()
    {
        [FaceState.Awake] = new BitmapImage(new Uri("pack://application:,,,/Assets/face.png")),
        [FaceState.Asleep] = new BitmapImage(new Uri("pack://application:,,,/Assets/face_sleep.png")),
    };

    private readonly CalculatorEngine _engine = new();

    /// <summary>Raised when the always-on-top toggle's checked state changes.</summary>
    public event Action<bool>? AlwaysOnTopChanged;

    public CalculatorView()
    {
        InitializeComponent();
        WireButtons();
        WireKeyboard();
        WireClipboard();
        WireAlwaysOnTop();
        RefreshDisplay();

        Loaded += (_, _) => Focus();
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

        // Speed-dial shortcuts: identical in effect to typing the digits
        // individually (see add-speed-dial-buttons).
        Btn45.Click += (_, _) => Handle(() => InputDigits("45"));
        Btn90.Click += (_, _) => Handle(() => InputDigits("90"));
        Btn180.Click += (_, _) => Handle(() => InputDigits("180"));
    }

    /// <summary>
    /// Keyboard input mirrors the button grid. Printable characters (digits,
    /// '.', the four operators, and '@' for square root - Windows
    /// Calculator's own shortcut) arrive via TextInput regardless of
    /// keyboard layout or whether they came from the top row or numpad, so
    /// they're handled in one place. Non-printable keys (Enter, Escape,
    /// Delete, Backspace, F9 for +/-, Ctrl+C/Ctrl+V) have no reliable
    /// character form and are handled via PreviewKeyDown instead. Both
    /// handlers are attached here (not to a child control) so they fire
    /// regardless of which button last had focus, since key events bubble.
    /// </summary>
    private void WireKeyboard()
    {
        PreviewTextInput += OnPreviewTextInput;
        PreviewKeyDown += OnPreviewKeyDown;
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        foreach (var ch in e.Text)
        {
            if (char.IsAsciiDigit(ch)) Handle(() => _engine.InputDigit(ch));
            else if (ch == '.') Handle(_engine.InputDecimalPoint);
            else if (ch is '+' or '-' or '*' or '/') Handle(() => _engine.InputOperator(ch));
            else if (ch == '@') Handle(_engine.SquareRoot);
        }
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

        switch (e.Key)
        {
            case Key.C when ctrl: CopyDisplay(); break;
            case Key.V when ctrl: PasteDisplay(); break;
            case Key.Enter: Handle(_engine.Equals); break;
            case Key.Escape: Handle(_engine.Clear); break;
            case Key.Delete: Handle(_engine.ClearEntry); break;
            case Key.Back: Handle(_engine.Backspace); break;
            case Key.F9: Handle(_engine.ToggleSign); break;
        }
    }

    /// <summary>
    /// Wires Ctrl+C/Ctrl+V and the display's right-click context menu to the
    /// same copy/paste code paths.
    /// </summary>
    private void WireClipboard()
    {
        CopyMenuItem.Click += (_, _) => CopyDisplay();
        PasteMenuItem.Click += (_, _) => PasteDisplay();
    }

    private void CopyDisplay()
    {
        try
        {
            Clipboard.SetText(_engine.Display);
        }
        catch (System.Runtime.InteropServices.ExternalException)
        {
            // The clipboard can be transiently locked by another process;
            // there's nothing useful to do beyond leaving it unchanged.
        }
    }

    private void PasteDisplay()
    {
        string text;
        try
        {
            if (!Clipboard.ContainsText()) return;
            text = Clipboard.GetText();
        }
        catch (System.Runtime.InteropServices.ExternalException)
        {
            return;
        }

        Handle(() => _engine.PasteValue(text));
    }

    /// <summary>
    /// The toggle only tells its host that always-on-top should change; it
    /// never touches <see cref="Window"/> itself (see class remarks).
    /// </summary>
    private void WireAlwaysOnTop()
    {
        BtnAlwaysOnTop.Checked += (_, _) => AlwaysOnTopChanged?.Invoke(true);
        BtnAlwaysOnTop.Unchecked += (_, _) => AlwaysOnTopChanged?.Invoke(false);
    }

    /// <summary>
    /// Called by the hosting window when its focus state changes (see
    /// <see cref="MainWindow.WindowFocusChanged"/>). Swaps the face to the
    /// sleep sprite while unfocused and back to normal on refocus.
    /// </summary>
    public void SetFocused(bool focused)
    {
        SetFaceState(focused ? FaceState.Awake : FaceState.Asleep);
    }

    private void SetFaceState(FaceState state)
    {
        FaceImage.Source = FaceStateAssets[state];
    }

    /// <summary>Runs an engine action, then refreshes the display.</summary>
    private void Handle(Action action)
    {
        action();
        RefreshDisplay();
    }

    /// <summary>Feeds each character of <paramref name="digits"/> through the
    /// engine's digit-entry path in sequence, as if typed individually.</summary>
    private void InputDigits(string digits)
    {
        foreach (var digit in digits)
        {
            _engine.InputDigit(digit);
        }
    }

    private void RefreshDisplay()
    {
        DisplayText.Text = _engine.Display;
    }
}
