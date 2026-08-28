## Context

Standard 4-function calculator semantics (as in Windows Calculator's basic
mode), implemented as a UI-independent class so it's unit-testable and so the
UI layer (`CalculatorView.xaml.cs`) stays a thin adapter.

## Goals / Non-Goals

**Goals:**
- `CalculatorEngine` has zero WPF dependencies - constructible and testable
  from a plain xunit test project.
- Standard chaining behavior: `5 + 3 + 2 =` behaves as `(5+3)+2=10`, matching
  everyday calculator expectations (immediate evaluation, not operator
  precedence / expression parsing).
- Predictable, non-crashing edge cases: divide by zero, sqrt of a negative,
  repeated `=`, chaining right after `=`, multiple decimal points.

**Non-Goals:**
- No expression parsing / operator precedence (this isn't a scientific
  calculator).
- No memory functions, percentage, or history - out of scope for this change.

## Decisions

- **State model**: `CalculatorEngine` keeps `_currentEntry` (string, what's
  being typed/shown), `_accumulator` (double?, the running value from a
  previous operation), `_pendingOperator` (char?), and `_startNewEntry`
  (bool - true right after an operator/equals/clear, so the next digit
  replaces rather than appends). Also `_lastOperator`/`_lastOperand` to
  support repeating `=` with no operator pressed in between (standard
  calculator behavior: pressing `=` again repeats the last operation).
- **Display formatting**: values are formatted via `"G15"` and trimmed, which
  avoids both floating-point noise (e.g. `0.1 + 0.2` showing
  `0.30000000000000004`) and unnecessary trailing zeros, while still showing
  up to 15 significant digits.
- **Error state**: a dedicated `IsError` flag. Divide-by-zero or sqrt of a
  negative number sets it and `Display` returns `"Error"`. While in error,
  only `Clear()` (C) is accepted; every other input is a no-op. This matches
  the CE-button requirements note ("C recovers from an error state") while
  keeping the error-state transition table small and easy to test.
- **CE vs C**: `ClearEntry()` resets just `_currentEntry` to `"0"` (and clears
  `IsError`, since there's no meaningful "current entry" to preserve while
  erroring), leaving `_accumulator`/`_pendingOperator` intact so an in-progress
  chained calculation survives a CE. `Clear()` resets everything.
- **Square root of a negative number**: sets `IsError` rather than throwing or
  producing `NaN` in the display.
- **UI wiring**: `CalculatorView.xaml.cs` holds one `CalculatorEngine`
  instance, and every button's `Click` handler calls one engine method then
  sets `DisplayText.Text = _engine.Display`. No business logic lives in the
  code-behind beyond that.

## Risks / Trade-offs

- `"G15"` formatting is a pragmatic choice, not a full "smart" calculator
  formatter (no scientific notation cutover, no locale-aware separators).
  Acceptable for v1; can be revisited in `add-app-polish` if it looks wrong
  in practice.
