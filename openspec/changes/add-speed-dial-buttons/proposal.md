## Why

The 45/90/180 speed-dial buttons are already laid out and styled in
`CalculatorView.xaml` (`Btn45`, `Btn90`, `Btn180`) but have no `Click`
handlers, so pressing them does nothing. This change wires them up.

## What Changes

- Wire `Btn45`, `Btn90`, and `Btn180` in `CalculatorView.xaml.cs` so pressing
  one feeds each character of its label through `CalculatorEngine.InputDigit`
  in sequence, then refreshes the display - identical to the user typing
  those digits one at a time.

## Capabilities

### New Capabilities
- (none)

### Modified Capabilities
- `calculator-engine`: adds a requirement covering the speed-dial buttons as
  a UI-level shortcut over the existing digit-entry path.

## Impact

- Modified: `Controls/CalculatorView.xaml.cs` (adds the three Click
  handlers). No engine changes, no XAML changes - the buttons and their
  styles already exist.
