## 1. Assets and button skins

- [x] 1.1 Copy `title_bar.png`, `title_bar_pressed.png`, `logo_button.png`,
  `logo_button_pressed.png`, `daisy.png`, `minimize_button.png`,
  `minimize_button_pressed.png`, `maximize_button.png`,
  `maximize_button_pressed.png`, `restore_button.png`,
  `restore_button_pressed.png`, `close_button.png`, `close_button_pressed.png`
  into `src/CuteGreenCalculator/Assets/` (picked up automatically by the
  existing `Assets\**\*.png` wildcard in the csproj).
- [x] 1.2 In `PixelButtonStyles.xaml`, add 72x56 (9x7 native x8) styles:
  `LogoButtonStyle`, `MinimizeButtonStyle`, `MaximizeButtonStyle`,
  `RestoreButtonStyle`, `CloseButtonStyle`, each wired to its Tag/PressedSource
  art via the existing `PixelButtonTemplate` pattern.

## 2. TitleBarView control

- [x] 2.1 Create `Controls/TitleBarView.xaml` (496x56 design grid): background
  `title_bar.png`/`title_bar_pressed.png` image, logo button with the daisy
  overlay on the left, app title text, and minimize/maximize-restore/close
  buttons right-aligned.
- [x] 2.2 In `TitleBarView.xaml.cs`: wire minimize/close to the hosting
  window's `WindowState`/`Close()`; wire the maximize/restore button to
  toggle `WindowState` and swap its own `Style` between
  `MaximizeButtonStyle`/`RestoreButtonStyle`, keeping it in sync via the
  window's `StateChanged` event; wire the non-button strip area to
  `DragMove()` on single-click (with the pressed background swapped in while
  held) and maximize/restore toggle on double-click; raise a
  `ResetRequested` event from the logo button's `Click`.

## 3. Borderless MainWindow

- [x] 3.1 In `MainWindow.xaml`: `WindowStyle="None"`, add `WindowChrome`
  (`CaptionHeight="0"`, `ResizeBorderThickness`, `GlassFrameThickness="0"`,
  `UseAeroCaptionButtons="False"`), set `Background` to `#7B8A5E`, bump
  `Width`/`Height` to 496/896 and `MinWidth`/`MinHeight` to 248/448, and
  replace the direct `CalculatorView` content with one `Viewbox` wrapping a
  496x896 grid stacking `TitleBarView` (row 0, 56px) above `CalculatorView`
  (row 1, 840px).
- [x] 3.2 In `MainWindow.xaml.cs`: update the `WM_SIZING` aspect-ratio
  constant to 496.0/896.0; wire `TitleBar.ResetRequested` to
  `Calculator.ResetDisplay()`.
- [x] 3.3 In `CalculatorView.xaml`, remove the now-redundant internal
  `Viewbox` wrapper (root becomes the 496x840 `Grid` directly).
- [x] 3.4 In `CalculatorView.xaml.cs`, add a public `ResetDisplay()` method
  wrapping the existing `Handle(_engine.Clear)` path.

## 4. Verification

- [x] 4.1 `dotnet build` succeeds with no warnings/errors.
- [x] 4.2 `dotnet test` passes with no regressions.
- [x] 4.3 Launch the app and confirm via UI Automation + a DPI-aware
  screenshot: no native title bar is visible, the title bar strip, logo,
  title text, and three window-control buttons render correctly, and the
  calculator face below is unchanged.
- [x] 4.4 Verify drag-to-move, edge/corner resize (aspect ratio still holds),
  double-click maximize/restore, and the minimize/maximize/close buttons all
  work, and that the maximize icon swaps to restore (and back) after each.
- [x] 4.5 Verify clicking the logo/daisy button resets the display the same
  way `C` does.

## 5. True-transparency maximized letterbox (review follow-up)

- [x] 5.1 In `MainWindow.xaml`, change `Background` from `#7B8A5E` to
  `Transparent` and add `AllowsTransparency="True"`.
- [x] 5.2 Re-verify (see design.md for method/results): interactive
  `WM_SYSCOMMAND`/`SC_SIZE` edge-drag resize still holds the aspect ratio, a
  raw-mouse drag on the title bar's `DragZone` still moves the window,
  maximize/restore still toggle correctly, and `dotnet build`/`dotnet test`
  stay clean (39/39 passing).
- [x] 5.3 Confirm via a real desktop screenshot (not `PrintWindow`) that the
  maximized letterbox genuinely shows the desktop through it.
