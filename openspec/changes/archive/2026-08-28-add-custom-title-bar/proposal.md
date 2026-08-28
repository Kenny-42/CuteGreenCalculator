## Why

Issue #16 asks for a custom pixel-art title bar in place of the native Windows
title bar, matching the app's hand-drawn aesthetic (logo, app name, and
min/max/restore/close controls skinned like every other button in the app).
The window needs to grow taller to fit this new strip without touching the
existing calculator face art. This was intentionally sequenced after issue
#15 (resizable window + `Viewbox` scaling), which the borderless chrome
depends on.

## What Changes

- New `TitleBarView` `UserControl` (window-chrome-aware, unlike
  `CalculatorView`): renders the title bar strip (`title_bar.png` /
  `title_bar_pressed.png`), the logo button (`logo_button.png` with the
  `daisy.png` flower overlaid on top), the app title text, and
  minimize/maximize-restore/close buttons skinned from the new button art,
  each with pressed states.
- `MainWindow` goes borderless: `WindowStyle="None"` with `WindowChrome`
  (`CaptionHeight="0"`, a `ResizeBorderThickness` for edge/corner resize) so
  the native frame disappears but edge/corner resizing still works; the
  window's own content becomes a single `Viewbox` wrapping a 496x896 grid
  stacking `TitleBarView` (56px strip) above `CalculatorView` (unchanged
  840px face), so both scale together as one unit. `CalculatorView` drops
  its own internal `Viewbox` since `MainWindow` now owns the one and only
  outer `Viewbox`.
- The aspect-ratio lock (`WM_SIZING` hook, from issue #15) is retargeted to
  496:896 (was 496:840) to include the new title bar strip; `MinWidth`/
  `MinHeight` scale proportionally (248/448).
- Title bar behavior: dragging the strip (outside the buttons) moves the
  window via `DragMove()`, showing `title_bar_pressed.png` while held;
  double-clicking the strip toggles maximize/restore; the minimize, maximize/
  restore, and close buttons perform the equivalent native window actions;
  the maximize button's art swaps to the restore icon (and back) whenever
  `WindowState` changes, from any source (button, double-click, Windows
  Snap).
- Clicking the logo/daisy button resets the calculator, identical to the `C`
  button - `CalculatorView` gains a small public `ResetDisplay()` method for
  this, and `TitleBarView` raises a `ResetRequested` event that `MainWindow`
  wires to it, following the same one-directional event pattern already used
  for the always-on-top toggle.
- New button skins added to `PixelButtonStyles.xaml` (logo, minimize,
  maximize, restore, close - each 72x56, the app's standard 8x scale of
  their native 9x7 art).
- No changes to `CalculatorEngine` or existing calculator button behavior.

## Capabilities

### New Capabilities
- `window-chrome`: custom borderless title bar - drag-to-move, resize,
  minimize/maximize/restore/close, and the logo reset button.

### Modified Capabilities
- `app-shell`: window is now borderless (`WindowStyle="None"` +
  `WindowChrome`) instead of using the native title bar; overall window
  height and aspect ratio change to include the title bar strip.
- `calculator-layout`: `CalculatorView` no longer owns the outer `Viewbox` -
  `MainWindow` does, wrapping both `TitleBarView` and `CalculatorView`
  together.

## Impact

- `src/CuteGreenCalculator/MainWindow.xaml` / `MainWindow.xaml.cs` (borderless
  chrome, `WindowChrome`, updated aspect ratio/min size, wiring
  `ResetRequested`).
- `src/CuteGreenCalculator/Controls/CalculatorView.xaml` (drop internal
  `Viewbox`) / `CalculatorView.xaml.cs` (new `ResetDisplay()` method).
- New `src/CuteGreenCalculator/Controls/TitleBarView.xaml` /
  `TitleBarView.xaml.cs`.
- `src/CuteGreenCalculator/Styles/PixelButtonStyles.xaml` (new title bar
  button skins).
- New assets copied into `src/CuteGreenCalculator/Assets/`: `title_bar.png`,
  `title_bar_pressed.png`, `logo_button.png`, `logo_button_pressed.png`,
  `daisy.png`, `minimize_button.png`, `minimize_button_pressed.png`,
  `maximize_button.png`, `maximize_button_pressed.png`, `restore_button.png`,
  `restore_button_pressed.png`, `close_button.png`, `close_button_pressed.png`.
