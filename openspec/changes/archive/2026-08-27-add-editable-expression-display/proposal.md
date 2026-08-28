## Why

GitHub issue #17 asks for the calculator display to become a real editable,
copyable text field instead of a static label: users should be able to click
into it, move the cursor, select and copy text, and edit the expression
directly, with the font auto-shrinking so long expressions never clip.

## What Changes

- **`CalculatorEngine`'s model changes from "the current operand only" to
  "the whole editable expression string"** (e.g. `12+34`, not just the last
  number typed) - the largest logic change in the project so far. A new
  caret-aware `TryInsert(selectionStart, selectionLength, insertText, out
  newCaretIndex)` is the single path every edit goes through (typed
  characters, pasted text, and button clicks alike), validated against a
  lightweight in-progress-expression grammar (digits, one decimal point per
  number, operators only after a number has started, `-` also allowed as a
  leading sign). A new `SetText(string)` resyncs engine state after an edit
  the UI already applied natively (Backspace/Delete/Cut in the TextBox).
  Unary operations (`+/-`, `√`) and `CE` now act on the trailing operand only,
  in place, rather than the whole entry.
- The older append-only API (`InputDigit`, `InputDecimalPoint`,
  `InputOperator`, `Backspace`, `PasteValue`) is kept as a thin wrapper over
  `TryInsert`/`SetText` so all 18 pre-existing tests keep passing unchanged.
- The display `TextBlock` is replaced with a real, styled `TextBox`: blinking
  caret, click/arrow-key cursor placement, mouse/keyboard text selection.
  Typed characters are filtered to calculator-understood characters only
  (digits, `.`, `+ - * /`, and `@` for `√`) via `PreviewTextInput`, rejecting
  everything else - including on paste, which is routed through the same
  filtering + `TryInsert` rather than a raw clipboard drop. Native
  Backspace/Delete/Cut edit the TextBox directly and resync through
  `SetText`. Existing shortcuts (Enter=`=`, Escape=`C`, F9=`+/-`, Ctrl+C
  copy, Ctrl+V paste) keep working; the Delete key stops being a `CE`
  shortcut so it can do its ordinary forward-delete job in a now-editable
  field (`CE` remains available via its button).
- The display auto-shrinks its font size (measured against the fixed-width
  output screen) as the expression grows, rather than clipping/ellipsizing.
- A new copy icon button (user-supplied `copy_button.png` /
  `copy_button_pressed.png`) sits in the top-left corner of the output
  screen and performs the same copy action as Ctrl+C/right-click copy
  (selection if any, else the whole display value).

## Capabilities

### Modified Capabilities
- `calculator-engine`: the display model becomes a full editable expression
  string with caret-aware insertion; unary/CE operations retarget to the
  trailing operand.
- `calculator-layout`: the output screen hosts an editable TextBox and a new
  copy icon button instead of a static label.
- `input-and-window-controls`: keyboard/paste handling adapts to a real
  editable text field (character filtering, native Backspace/Delete/Cut,
  Delete key no longer double-booked as `CE`).

## Impact

- `src/CuteGreenCalculator/CalculatorEngine.cs`: rewritten around a single
  editable expression string; new `TryInsert`/`SetText`, retargeted
  `ToggleSign`/`SquareRoot`/`ClearEntry`.
- `src/CuteGreenCalculator/Controls/CalculatorView.xaml` /
  `CalculatorView.xaml.cs`: TextBox display, copy button, caret-aware button
  wiring, auto-shrink font sizing, revised keyboard/clipboard handling.
- `src/CuteGreenCalculator/Styles/PixelButtonStyles.xaml`: a small icon
  button style for the copy button.
- `src/CuteGreenCalculator/Assets/copy_button.png` /
  `copy_button_pressed.png`: new user-supplied art.
- `tests/CuteGreenCalculator.Tests/CalculatorEngineTests.cs`: new tests for
  mid-string edits, selection replacement, and grammar rejection.
