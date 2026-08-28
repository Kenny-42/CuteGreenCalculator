using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CuteGreenCalculator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Native aspect ratio of the title bar + calculator face together
    // (496x896: the 56px title bar strip on top of background.png at its 8x
    // integer scale, 496x840). Kept locked during resize so the pixel art is
    // only ever scaled uniformly, never stretched - see add-resizable-window
    // and, for the title bar addition, issue #16. Not enforced when
    // maximizing (WM_SIZING isn't sent for that transition) - see design.md.
    private const double AspectRatio = 496.0 / 896.0;

    private const int WM_SIZING = 0x0214;
    private const int WMSZ_LEFT = 1;
    private const int WMSZ_RIGHT = 2;
    private const int WMSZ_TOP = 3;
    private const int WMSZ_TOPLEFT = 4;
    private const int WMSZ_TOPRIGHT = 5;
    private const int WMSZ_BOTTOM = 6;
    private const int WMSZ_BOTTOMLEFT = 7;
    private const int WMSZ_BOTTOMRIGHT = 8;

    public MainWindow()
    {
        InitializeComponent();
        Calculator.AlwaysOnTopChanged += isAlwaysOnTop => Topmost = isAlwaysOnTop;
        SourceInitialized += OnSourceInitialized;

        // Face sleep state (issue #18): CalculatorView never touches Window
        // itself, so MainWindow drives it directly here, mirroring the
        // always-on-top wiring above but in the opposite direction.
        Activated += (_, _) => Calculator.SetFocused(true);
        Deactivated += (_, _) => Calculator.SetFocused(false);

        // Title bar logo/daisy button resets the calculator (issue #16) -
        // TitleBarView raises a plain event rather than referencing
        // CalculatorView directly, same one-directional pattern as above.
        TitleBar.ResetRequested += () => Calculator.ResetDisplay();
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        if (PresentationSource.FromVisual(this) is HwndSource source)
        {
            source.AddHook(WndProc);
        }
    }

    /// <summary>
    /// Intercepts WM_SIZING to constrain the drag rectangle to the calculator
    /// face's native aspect ratio, so dragging any edge/corner resizes the
    /// window without ever distorting the pixel art (Viewbox in
    /// CalculatorView handles the actual uniform scaling once the window
    /// itself is the right shape).
    /// </summary>
    private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_SIZING)
        {
            var rect = Marshal.PtrToStructure<RECT>(lParam);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            int edge = wParam.ToInt32();

            switch (edge)
            {
                // Dragging a purely vertical edge (top/bottom): width follows height.
                case WMSZ_TOP:
                case WMSZ_BOTTOM:
                    width = (int)Math.Round(height * AspectRatio);
                    rect.Right = rect.Left + width;
                    break;

                // Dragging a purely horizontal edge (left/right): height follows width.
                case WMSZ_LEFT:
                case WMSZ_RIGHT:
                    height = (int)Math.Round(width / AspectRatio);
                    rect.Bottom = rect.Top + height;
                    break;

                // Corners: keep the larger of the two proposed deltas driving
                // the ratio, and adjust from the edge(s) being dragged.
                default:
                    if (width / AspectRatio > height)
                    {
                        height = (int)Math.Round(width / AspectRatio);
                        if (edge is WMSZ_TOPLEFT or WMSZ_TOPRIGHT)
                        {
                            rect.Top = rect.Bottom - height;
                        }
                        else
                        {
                            rect.Bottom = rect.Top + height;
                        }
                    }
                    else
                    {
                        width = (int)Math.Round(height * AspectRatio);
                        if (edge is WMSZ_TOPLEFT or WMSZ_BOTTOMLEFT)
                        {
                            rect.Left = rect.Right - width;
                        }
                        else
                        {
                            rect.Right = rect.Left + width;
                        }
                    }
                    break;
            }

            Marshal.StructureToPtr(rect, lParam, true);
            handled = true;
        }

        return IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
