## Why

Issue #24: the 4 hearts in the status row are currently purely decorative
static images. New `heart_pressed.png` art exists, so they can become a
clickable threshold toggle group (a life/level-style indicator) instead.

## What Changes

- New `HeartToggleStyle` in `PixelButtonStyles.xaml` (a `ToggleButton`,
  reusing the existing `PixelToggleButtonTemplate`/`CheckedSource` pattern
  from the always-on-top pin toggle), skinned with `heart.png` (normal) /
  `heart_pressed.png` (checked), sized 56x48 to match the hearts' existing
  rendered size.
- The 4 static heart `Image` elements in `CalculatorView.xaml` become 4
  named `ToggleButton`s (`Heart0`..`Heart3`).
- New group-threshold logic in `CalculatorView.xaml.cs`
  (`WireHearts`/`OnHeartClicked`/`RefreshHearts`): clicking a heart toggles
  it and every heart to its right on, untoggling everything to its left -
  i.e. sets a single `_heartThreshold` index that every heart's `IsChecked`
  is recomputed from on every click, overwriting whatever `ToggleButton`'s
  own default click-to-toggle behavior just did. Clicking the leftmost
  currently-toggled heart again clears the whole group back to off.
- New art asset: `Assets/heart_pressed.png` (real user-supplied sprite, from
  the same pixilart asset source as `heart.png`).
- Purely visual/decorative - no `CalculatorEngine` involvement.

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
- `calculator-layout`: the heart display is now an interactive threshold
  toggle group instead of 4 static images.

## Impact

- `src/CuteGreenCalculator/Styles/PixelButtonStyles.xaml` (new
  `HeartToggleStyle`).
- `src/CuteGreenCalculator/Controls/CalculatorView.xaml` /
  `CalculatorView.xaml.cs` (hearts become named `ToggleButton`s, new
  threshold-toggle wiring).
- New asset: `src/CuteGreenCalculator/Assets/heart_pressed.png`.
- No changes to `CalculatorEngine` or the existing button wiring.
