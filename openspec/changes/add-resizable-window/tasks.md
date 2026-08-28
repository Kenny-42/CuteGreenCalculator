## 1. Resizable window + aspect ratio lock

- [x] 1.1 In `MainWindow.xaml`, set `ResizeMode="CanResize"`, remove `SizeToContent="WidthAndHeight"`, add `MinWidth="248"` and `MinHeight="420"`, and set explicit `Width`/`Height` (496/840) as the startup size.
- [x] 1.2 In `MainWindow.xaml.cs`, hook `HwndSource` (via `SourceInitialized`) and intercept `WM_SIZING` to constrain the resize rectangle to the 496:840 aspect ratio based on which edge/corner is being dragged.

## 2. Uniform face scaling

- [x] 2.1 In `CalculatorView.xaml`, wrap the existing `Grid Width="496" Height="840"` in a `Viewbox Stretch="Uniform"` so the whole face scales as one unit with the window.

## 3. Button row alignment fix

- [x] 3.1 In `CalculatorView.xaml`, change the standard 4-column button row margins from `0,0,8,0` to `0,0,16,0` (rows: `C CE +/- √`, `7 8 9 ÷`, `4 5 6 ×`, `1 2 3 −`, `0 . + =`) so each row totals 432px, matching the output screen width.
- [x] 3.2 Change the speed-dial row (`45 90 180`) margins from `0,0,8,0` to `0,0,16,0` so it also totals 432px.

## 4. Verification

- [x] 4.1 `dotnet build` succeeds with no warnings/errors.
- [x] 4.2 `dotnet test` passes (no engine/logic changes expected, but confirm no regressions) - 25/25 passed.
- [x] 4.3 Launch the app and drag-resize via a real interactive resize loop (`WM_SYSCOMMAND`/`SC_SIZE` + simulated mouse drag, not just `MoveWindow`, since only an interactive drag sends `WM_SIZING`): resulting window ratio measured 0.5904 vs. target 0.5905 (496/840) - aspect lock holds under a genuine drag.
- [x] 4.4 DPI-aware screenshots at native size and after a resize confirm button rows align with the output screen's left/right edges and the face scales without distortion.
