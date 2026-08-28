using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
/// digit/operator/function/equals button to it, including the 45/90/180/270
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

    private const double MaxDisplayFontSize = 32;
    private const double MinDisplayFontSize = 14;
    private const double DisplayAvailableWidth = 432 - 84; // matches DisplayText's Padding (64 left to clear the copy button, 20 right)

    private readonly CalculatorEngine _engine = new();

    // Guards DisplayText's TextChanged handler against reacting to Text
    // assignments this class itself makes (RefreshDisplay, or reverting a
    // rejected native edit) - see OnDisplayTextChanged.
    private bool _syncingText;

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

        Loaded += (_, _) => DisplayText.Focus();
    }

    /// <summary>
    /// Every digit/operator/decimal/speed-dial button inserts its text at
    /// DisplayText's current caret position/selection - the same path typed
    /// and pasted characters use (see <see cref="OnPreviewTextInput"/>) -
    /// so clicking a button edits wherever the cursor was last placed, not
    /// always the end.
    /// </summary>
    private void WireButtons()
    {
        Btn0.Click += (_, _) => InsertAtCaret("0");
        Btn1.Click += (_, _) => InsertAtCaret("1");
        Btn2.Click += (_, _) => InsertAtCaret("2");
        Btn3.Click += (_, _) => InsertAtCaret("3");
        Btn4.Click += (_, _) => InsertAtCaret("4");
        Btn5.Click += (_, _) => InsertAtCaret("5");
        Btn6.Click += (_, _) => InsertAtCaret("6");
        Btn7.Click += (_, _) => InsertAtCaret("7");
        Btn8.Click += (_, _) => InsertAtCaret("8");
        Btn9.Click += (_, _) => InsertAtCaret("9");

        BtnDecimal.Click += (_, _) => InsertAtCaret(".");

        BtnAdd.Click += (_, _) => InsertAtCaret("+");
        BtnSubtract.Click += (_, _) => InsertAtCaret("-");
        BtnMultiply.Click += (_, _) => InsertAtCaret("*");
        BtnDivide.Click += (_, _) => InsertAtCaret("/");

        BtnEquals.Click += (_, _) => Handle(_engine.Equals);
        BtnClear.Click += (_, _) => Handle(_engine.Clear);
        BtnClearEntry.Click += (_, _) => Handle(_engine.ClearEntry);
        BtnSign.Click += (_, _) => Handle(_engine.ToggleSign);
        BtnSqrt.Click += (_, _) => Handle(_engine.SquareRoot);

        // Speed-dial shortcuts: identical in effect to typing the digits
        // individually (see add-speed-dial-buttons).
        Btn45.Click += (_, _) => InsertAtCaret("45");
        Btn90.Click += (_, _) => InsertAtCaret("90");
        Btn180.Click += (_, _) => InsertAtCaret("180");
        Btn270.Click += (_, _) => InsertAtCaret("270");

        BtnCopyDisplay.Click += (_, _) => CopyDisplay();
    }

    /// <summary>
    /// Keyboard input mirrors the button grid. Printable characters (digits,
    /// '.', the four operators, and '@' for square root - Windows
    /// Calculator's own shortcut) arrive via TextInput regardless of
    /// keyboard layout or whether they came from the top row or numpad, so
    /// they're handled in one place: every character is filtered and, if
    /// accepted, applied through the same caret-aware
    /// <see cref="CalculatorEngine.TryInsert"/> button clicks use -
    /// <see cref="TextCompositionEventArgs.Handled"/> is always set so
    /// DisplayText's own default insertion never runs, avoiding a double
    /// edit or letting an unsupported character slip through. Non-printable
    /// keys (Enter, Escape, F9 for +/-, Ctrl+C/Ctrl+V) have no reliable
    /// character form and are handled via PreviewKeyDown instead. Both
    /// handlers are attached here (not to DisplayText) so they fire
    /// regardless of which button last had focus, since key events tunnel
    /// through this control before reaching whichever child has focus.
    /// Backspace, Delete, and Cut are deliberately left to DisplayText's own
    /// native handling (see <see cref="OnDisplayTextChanged"/>) now that
    /// it's a real editable field.
    /// </summary>
    private void WireKeyboard()
    {
        PreviewTextInput += OnPreviewTextInput;
        PreviewKeyDown += OnPreviewKeyDown;
        DisplayText.TextChanged += OnDisplayTextChanged;
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = true;
        foreach (var ch in e.Text)
        {
            if (ch == '@')
            {
                Handle(_engine.SquareRoot);
            }
            else if (CalculatorEngine.IsAllowedChar(ch))
            {
                InsertAtCaret(ch.ToString());
            }
        }
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

        switch (e.Key)
        {
            case Key.C when ctrl: CopyDisplay(); e.Handled = true; break;
            case Key.V when ctrl: PasteDisplay(); e.Handled = true; break;
            case Key.Enter: Handle(_engine.Equals); e.Handled = true; break;
            case Key.Escape: Handle(_engine.Clear); e.Handled = true; break;
            case Key.F9: Handle(_engine.ToggleSign); e.Handled = true; break;
        }
    }

    /// <summary>
    /// Resyncs the engine from a native edit DisplayText already applied to
    /// its own Text (Backspace, Delete, Cut, or Undo) - anything that isn't
    /// character-insertion, which <see cref="OnPreviewTextInput"/> already
    /// owns. Reverts the TextBox back to the engine's text if the resync is
    /// rejected (e.g. some exotic edit produced an illegal expression).
    /// </summary>
    private void OnDisplayTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_syncingText || _engine.IsError) return;

        if (_engine.SetText(DisplayText.Text))
        {
            AutoSizeDisplayFont();
            return;
        }

        var caret = DisplayText.CaretIndex;
        _syncingText = true;
        DisplayText.Text = _engine.Display;
        DisplayText.CaretIndex = Math.Min(caret, DisplayText.Text.Length);
        _syncingText = false;
    }

    /// <summary>
    /// Wires the display's right-click context menu to the same copy/paste
    /// code paths as Ctrl+C/Ctrl+V and the copy button.
    /// </summary>
    private void WireClipboard()
    {
        CopyMenuItem.Click += (_, _) => CopyDisplay();
        PasteMenuItem.Click += (_, _) => PasteDisplay();
    }

    /// <summary>Copies the current selection, or the whole display value if nothing is selected.</summary>
    private void CopyDisplay()
    {
        var text = DisplayText.SelectionLength > 0 ? DisplayText.SelectedText : _engine.Display;
        try
        {
            Clipboard.SetText(text);
        }
        catch (System.Runtime.InteropServices.ExternalException)
        {
            // The clipboard can be transiently locked by another process;
            // there's nothing useful to do beyond leaving it unchanged.
        }
    }

    /// <summary>
    /// Inserts clipboard text at the current caret/selection through the
    /// same filtered, caret-aware path as typing - unsupported characters
    /// are dropped rather than the paste being rejected outright.
    /// </summary>
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

        InsertAtCaret(text);
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

    /// <summary>
    /// Runs an engine action that isn't a caret-aware insertion (Equals,
    /// Clear, CE, +/-, √), then refreshes the display and moves the cursor
    /// to the end - these operations replace text out from under whatever
    /// the caret was previously doing, so "ready to keep typing at the end"
    /// is the only sensible place to leave it.
    /// </summary>
    private void Handle(Action action)
    {
        action();
        RefreshDisplay();
        DisplayText.CaretIndex = DisplayText.Text.Length;
        DisplayText.Focus();
    }

    /// <summary>
    /// Inserts <paramref name="text"/> at DisplayText's current caret
    /// position/selection via <see cref="CalculatorEngine.TryInsert"/>. Used
    /// by every digit/operator/decimal/speed-dial button, typed characters,
    /// and pasted text alike, so all of them edit wherever the cursor
    /// currently is rather than always appending at the end.
    /// </summary>
    private void InsertAtCaret(string text)
    {
        if (_engine.TryInsert(DisplayText.SelectionStart, DisplayText.SelectionLength, text, out var caret))
        {
            RefreshDisplay();
            DisplayText.CaretIndex = caret;
        }
        DisplayText.Focus();
    }

    private void RefreshDisplay()
    {
        _syncingText = true;
        DisplayText.Text = _engine.Display;
        _syncingText = false;
        DisplayText.IsReadOnly = _engine.IsError;
        AutoSizeDisplayFont();
    }

    /// <summary>
    /// Shrinks DisplayText's font size as the expression grows so it's
    /// never clipped, measuring against the output screen's fixed width. A
    /// plain code-behind calculation (no converter/behavior) matching this
    /// codebase's preference for direct code over MVVM machinery elsewhere.
    /// A fixed 1.0 "pixels per DIP" is used rather than the real display's
    /// DPI - this is only a relative size comparison, not pixel-perfect
    /// rendering, and sidesteps this app's known DPI-awareness quirks (see
    /// project notes on SetThreadDpiAwarenessContext).
    /// </summary>
    private void AutoSizeDisplayFont()
    {
        var text = string.IsNullOrEmpty(DisplayText.Text) ? "0" : DisplayText.Text;
        var typeface = new Typeface(DisplayText.FontFamily, DisplayText.FontStyle, DisplayText.FontWeight, DisplayText.FontStretch);

        var size = MaxDisplayFontSize;
        while (size > MinDisplayFontSize && MeasureTextWidth(text, typeface, size) > DisplayAvailableWidth)
        {
            size -= 1;
        }
        DisplayText.FontSize = size;
    }

    private static double MeasureTextWidth(string text, Typeface typeface, double fontSize)
    {
        var formatted = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            fontSize,
            Brushes.Black,
            pixelsPerDip: 1.0);
        return formatted.Width;
    }
}
