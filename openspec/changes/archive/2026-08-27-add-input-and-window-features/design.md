## Context

`CalculatorView` (a `UserControl`) deliberately has no knowledge of its
hosting window's chrome, so a future custom borderless frame can host it
unchanged. Keyboard input and clipboard access fit naturally inside the
view, but the always-on-top toggle is a window-level concern
(`Window.Topmost`) and must not break that separation.

## Goals / Non-Goals

**Goals:**
- Keyboard input reaches `CalculatorEngine` through the same code paths the
  buttons already use (`Handle(...)`).
- Copy/paste is discoverable (context menu) and fast (Ctrl+C/Ctrl+V).
- The always-on-top toggle works without `CalculatorView` referencing
  `Window` directly.
- Placeholder art for the toggle is isolated in one style so it's a one-line
  swap later.

**Non-Goals:**
- No custom borderless window frame (tracked separately, not this issue).
- No persistence of the always-on-top preference across app restarts.
- No full expression clipboard support (only the display's current numeric
  value is copied/pasted).

## Decisions

- **Text input via `PreviewTextInput`, control keys via `PreviewKeyDown`.**
  Digits, `.`, `+ - * /`, and `@` (√) all arrive as printable characters
  through `TextInput` regardless of keyboard layout or whether they came
  from the top row or numpad, so routing them through one character-based
  handler avoids duplicating logic per physical key (e.g. `Key.Add` vs.
  `Key.OemPlus`). Non-printable keys (Enter, Escape, Delete, Backspace, F9,
  Ctrl+C, Ctrl+V) have no reliable character form, so they're handled via
  `PreviewKeyDown` instead. Handlers are attached to `CalculatorView` itself
  (not a child control), so they fire regardless of which button last had
  focus, since key events bubble up.
- **F9 for +/- and @ for √.** These match Windows Calculator's own
  shortcuts, so they're a "sensible standard mapping" per the issue rather
  than an arbitrary choice.
- **`AlwaysOnTopChanged` event instead of `CalculatorView` touching
  `Window`.** `CalculatorView` raises a plain C# event when its toggle
  button's checked state changes; `MainWindow` subscribes and sets its own
  `Topmost`. This keeps `CalculatorView` chrome-agnostic - it never calls
  `Window.GetWindow(this)` or references `Window` at all - matching the
  existing design note in `CalculatorView.xaml`.
- **`ToggleButton`, not `Button`, for the always-on-top control.** A
  `ToggleButton` has built-in `IsChecked` state, so the pressed/unpressed
  visual indicator is free (an `IsChecked` trigger swapping art), rather
  than tracking a separate bool and manually swapping images.
- **Placeholder art isolated in one style.** A new
  `AlwaysOnTopToggleStyle` in `PixelButtonStyles.xaml` is the only place
  `heart.png`/`heart_screen.png` are referenced for this purpose, with an
  XML comment marking it temporary. Swapping in real art later means
  changing two `BitmapImage` URIs in one place.
- **`PasteValue` sanitizes via `double.TryParse`, not regex.** Anything
  that isn't a valid finite number (empty, letters, multiple decimal
  points, whitespace-only) is rejected by `TryParse` and silently ignored,
  satisfying "reject or ignore non-numeric garbage rather than crashing"
  without hand-rolled validation.

## Risks / Trade-offs

- [Ctrl+C/Ctrl+V could conflict with a future text-editable control such as
  an expression bar] → Not in scope now; the event handlers are scoped to
  `CalculatorView`'s key events and can be narrowed later if a real text
  input control is added.
- [`PasteValue` accepts values `CalculatorEngine` couldn't have produced
  itself, e.g. very long decimals] → Reuses the existing `FormatNumber`
  ("G15") path so display formatting stays consistent with everything
  else.
