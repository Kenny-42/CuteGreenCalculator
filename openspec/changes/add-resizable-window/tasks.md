## 1. Resizable window + aspect ratio lock

- [ ] 1.1 In `MainWindow.xaml`, set `ResizeMode="CanResize"`, remove `SizeToContent="WidthAndHeight"`, add `MinWidth="248"` and `MinHeight="420"`, and set explicit `Width`/`Height` (496/840) as the startup size.
- [ ] 1.2 In `MainWindow.xaml.cs`, hook `HwndSource` (via `SourceInitialized`) and intercept `WM_SIZING` to constrain the resize rectangle to the 496:840 aspect ratio based on which edge/corner is being dragged.

## 2. Uniform face scaling

- [ ] 2.1 In `CalculatorView.xaml`, wrap the existing `Grid Width="496" Height="840"` in a `Viewbox Stretch="Uniform"` so the whole face scales as one unit with the window.

## 3. Button row alignment fix

- [ ] 3.1 In `CalculatorView.xaml`, change the standard 4-column button row margins from `0,0,8,0` to `0,0,16,0` (rows: `C CE +/- √`, `7 8 9 ÷`, `4 5 6 ×`, `1 2 3 −`, `0 . + =`) so each row totals 432px, matching the output screen width.
- [ ] 3.2 Change the speed-dial row (`45 90 180`) margins from `0,0,8,0` to `0,0,16,0` so it also totals 432px.

## 4. Verification

- [ ] 4.1 `dotnet build` succeeds with no warnings/errors.
- [ ] 4.2 `dotnet test` passes (no engine/logic changes expected, but confirm no regressions).
- [ ] 4.3 Launch the app, drag-resize from each edge and corner, and confirm: no distortion, no letterbox bars, min size holds.
- [ ] 4.4 DPI-aware screenshot at native size confirms button rows now align with the output screen's left/right edges.
