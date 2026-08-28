## Why

The calculator is currently mouse-only and always floats behind other windows
like an ordinary app window. GitHub issue #5 asks for full keyboard support,
clipboard copy/paste of the display value, and an always-on-top toggle so it
can be used as a floating widget - closing the gap between this app and a
standard desktop calculator's usability baseline.

## What Changes

- Keyboard input drives the same `CalculatorEngine` paths as the on-screen
  buttons: digits, `.`, `+ - * /`, Enter/Return for `=`, Escape for `C`,
  Delete for `CE`, Backspace for single-character delete, F9 for `+/-`, and
  `@` for `√` (matching Windows Calculator's own key conventions for the two
  less-obvious ones).
- `CalculatorEngine` gains a `Backspace()` operation (removes the last
  entered character, or resets to `0`) and a `PasteValue(string)` operation
  (parses and accepts an external numeric string, silently ignoring anything
  that doesn't parse).
- The display value can be copied with Ctrl+C, or via a right-click context
  menu, and pasted with Ctrl+V or the same context menu; pasted text is
  validated through `PasteValue` so garbage input is ignored rather than
  crashing the app.
- A new always-on-top toggle button is added to the calculator face. It
  reuses `heart.png` (off) / `heart_screen.png` (on) as **TEMPORARY
  placeholder art** - clearly marked in code/XAML comments - until real
  sprite art exists. Toggling it sets/unsets the host window's `Topmost`,
  and the button's own checked/unchecked state is the visual indicator.

## Capabilities

### New Capabilities
- `input-and-window-controls`: keyboard-driven calculator input, clipboard
  copy/paste of the display, and an always-on-top window toggle.

### Modified Capabilities
- `calculator-engine`: adds `Backspace` (single-character delete) and
  `PasteValue` (validated external numeric input) operations.

## Impact

- `src/CuteGreenCalculator/CalculatorEngine.cs`: new `Backspace()` and
  `PasteValue(string)` methods.
- `src/CuteGreenCalculator/Controls/CalculatorView.xaml` /
  `CalculatorView.xaml.cs`: keyboard event wiring, context menu, always-on-top
  toggle button, an `AlwaysOnTopChanged` event so the chrome-agnostic view can
  ask its host window to change `Topmost` without referencing `Window`
  directly.
- `src/CuteGreenCalculator/MainWindow.xaml.cs`: subscribes to
  `AlwaysOnTopChanged` and sets `Topmost` on itself.
- `src/CuteGreenCalculator/Styles/PixelButtonStyles.xaml` /
  `PixelButtonProperties.cs`: a toggle-button style/template and a
  `CheckedSource` attached property for the placeholder always-on-top art.
- `tests/CuteGreenCalculator.Tests/CalculatorEngineTests.cs`: new tests for
  `Backspace` and `PasteValue`.
