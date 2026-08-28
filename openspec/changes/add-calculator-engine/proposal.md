## Why

The app currently just displays static art with no functionality. This change
makes it a genuinely usable calculator: a UI-independent arithmetic engine
wired to the digit/operator/function buttons and the display.

## What Changes

- Add `CalculatorEngine`, a plain C# class (no WPF types) implementing digit
  entry, chained binary operations (+, −, ×, ÷), unary operations (+/- sign
  toggle, √), C (full clear), CE (clear entry), and = (evaluate).
- Wire every non-speed-dial button added in `add-pixel-button-styles` to the
  engine, updating the display live.
- Add a test project with unit tests covering the engine's behavior and edge
  cases in isolation from the UI.

## Capabilities

### New Capabilities
- `calculator-engine`: Core arithmetic state machine — entry, chaining,
  unary ops, clear semantics, and error handling.

### Modified Capabilities
- (none - button Click wiring is covered under the new `calculator-engine`
  capability's requirements, since it's the engine that gives those clicks
  meaning; the visual layout requirements in `calculator-layout` are
  unchanged)

## Impact

- New: `src/CuteGreenCalculator/CalculatorEngine.cs`,
  `tests/CuteGreenCalculator.Tests/` (new test project), added to the `.sln`.
- Modified: `Controls/CalculatorView.xaml.cs` (Click handlers), possibly
  `CalculatorView.xaml` (x:Name additions if any are missing - none expected,
  all buttons already have names).
- Speed-dial buttons (45/90/180) are explicitly OUT of scope here - they're
  wired in `add-speed-dial-buttons`.
