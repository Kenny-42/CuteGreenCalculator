## 1. Engine: Backspace and PasteValue

- [x] 1.1 Add `Backspace()` to `CalculatorEngine` (removes last character;
  resets to `0` on a single digit or a lone minus sign; no-op on a fresh
  entry or in error state)
- [x] 1.2 Add `PasteValue(string)` to `CalculatorEngine` (parses with
  `double.TryParse`, ignores invalid input, otherwise replaces the current
  entry via the existing `FormatNumber` path)
- [x] 1.3 Add xunit tests for both in `CalculatorEngineTests.cs` covering
  the scenarios in `specs/calculator-engine/spec.md`
- [x] 1.4 `dotnet test` passes

## 2. Keyboard input wiring

- [x] 2.1 Make `CalculatorView` focusable and give it initial keyboard focus
  on load
- [x] 2.2 Handle `PreviewTextInput` for digits, `.`, `+ - * /`, and `@`
  (square root), routing each through the existing `Handle(...)` pattern
- [x] 2.3 Handle `PreviewKeyDown` for Enter (`=`), Escape (`C`), Delete
  (`CE`), Backspace, and F9 (`+/-`)
- [x] 2.4 Manually verify each mapping via a UI-automation-driven run
  (keyboard input can't be sent through `InvokePattern`, so send key events
  or use `SendKeys`/`SendInput`-equivalent, then screenshot with the DPI fix)

## 3. Copy/paste

- [x] 3.1 Handle Ctrl+C in `PreviewKeyDown` to copy `DisplayText.Text` to
  the clipboard
- [x] 3.2 Handle Ctrl+V in `PreviewKeyDown` to read clipboard text and pass
  it to `_engine.PasteValue`, then refresh the display
- [x] 3.3 Add a right-click `ContextMenu` on the output screen with "Copy"
  and "Paste" items wired to the same code paths
- [x] 3.4 Manually verify copy/paste round-trips a value and that pasting
  non-numeric clipboard text is silently ignored

## 4. Always-on-top toggle

- [x] 4.1 Add a `CheckedSource` attached property alongside the existing
  `PressedSource` on `PixelButton` (or a new attached-property class) for
  toggle-button art
- [x] 4.2 Add an `AlwaysOnTopToggleStyle` `ToggleButton` style in
  `PixelButtonStyles.xaml` using `heart.png` (unchecked) /
  `heart_screen.png` (checked), with a code comment marking it TEMPORARY
  placeholder art per issue #5
- [x] 4.3 Add the toggle button to `CalculatorView.xaml` in a spot that
  doesn't disturb the existing button grid layout
- [x] 4.4 Add a public `AlwaysOnTopChanged` event to `CalculatorView`,
  raised from the toggle's `Checked`/`Unchecked` handlers, so the view never
  references `Window` directly
- [x] 4.5 Subscribe to `AlwaysOnTopChanged` in `MainWindow.xaml.cs` and set
  `Topmost` accordingly
- [x] 4.6 Manually verify toggling changes both the button's visual state
  and the window's topmost behavior

## 5. Wrap-up

- [x] 5.1 Update the OpenSpec change notes/design if implementation
  deviated from the plan
- [x] 5.2 `dotnet build` and `dotnet test` both pass
- [x] 5.3 Open PR against `main` with "Closes #5", wait for CI, self-merge
  per project workflow
