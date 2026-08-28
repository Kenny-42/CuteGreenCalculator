using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CuteGreenCalculator.Controls;

/// <summary>
/// Custom borderless title bar (issue #16): drag-to-move, double-click
/// maximize/restore, and minimize/maximize/restore/close buttons - all
/// acting on the hosting <see cref="Window"/> directly, since driving the
/// window is this control's entire purpose (unlike <see cref="CalculatorView"/>,
/// which stays window-chrome-agnostic). The logo/reset button is the one
/// exception: it raises <see cref="ResetRequested"/> rather than reaching
/// into <see cref="CalculatorView"/> itself, so this control stays decoupled
/// from calculator logic specifically.
/// </summary>
public partial class TitleBarView : UserControl
{
    private static readonly BitmapImage NormalBackground =
        new(new Uri("pack://application:,,,/Assets/title_bar.png"));
    private static readonly BitmapImage PressedBackground =
        new(new Uri("pack://application:,,,/Assets/title_bar_pressed.png"));

    private Window? _window;

    /// <summary>Raised when the logo/daisy button is clicked - the calculator should reset, exactly as if `C` were clicked.</summary>
    public event Action? ResetRequested;

    public TitleBarView()
    {
        InitializeComponent();

        BtnLogo.Click += (_, _) => ResetRequested?.Invoke();
        BtnMinimize.Click += (_, _) => { if (_window != null) _window.WindowState = WindowState.Minimized; };
        BtnMaximizeRestore.Click += (_, _) => ToggleMaximizeRestore();
        BtnClose.Click += (_, _) => _window?.Close();

        DragZone.MouseLeftButtonDown += OnDragZoneMouseLeftButtonDown;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _window = Window.GetWindow(this);
        if (_window == null) return;

        _window.StateChanged += (_, _) => UpdateMaximizeRestoreButton();
        UpdateMaximizeRestoreButton();
    }

    /// <summary>
    /// Double-click toggles maximize/restore, matching standard title bar
    /// behavior; a single click drags the window via the same
    /// <see cref="Window.DragMove"/> every custom-chrome WPF app uses, which
    /// blocks until the mouse button is released - the pressed background
    /// art is shown for the duration.
    /// </summary>
    private void OnDragZoneMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_window == null) return;

        if (e.ClickCount == 2)
        {
            ToggleMaximizeRestore();
            return;
        }

        BackgroundImage.Source = PressedBackground;
        try
        {
            _window.DragMove();
        }
        catch (InvalidOperationException)
        {
            // DragMove throws if the mouse button was already released by
            // the time it's called (e.g. a very fast click) - nothing to do.
        }
        finally
        {
            BackgroundImage.Source = NormalBackground;
        }
    }

    private void ToggleMaximizeRestore()
    {
        if (_window == null) return;
        _window.WindowState = _window.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    /// <summary>
    /// Keeps the maximize/restore button's art in sync with the window's
    /// actual state, however that state changed (this button, double-click,
    /// or a Windows Snap gesture) - driven by Window.StateChanged rather than
    /// only the click handlers above.
    /// </summary>
    private void UpdateMaximizeRestoreButton()
    {
        if (_window == null) return;

        var isMaximized = _window.WindowState == WindowState.Maximized;
        BtnMaximizeRestore.Style = (Style)FindResource(isMaximized ? "RestoreButtonStyle" : "MaximizeButtonStyle");
        BtnMaximizeRestore.ToolTip = isMaximized ? "Restore" : "Maximize";
        AutomationProperties.SetName(BtnMaximizeRestore, isMaximized ? "Restore" : "Maximize");
    }
}
