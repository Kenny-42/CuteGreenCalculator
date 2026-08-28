## Why

Issue #18 asks for the top-left face to switch to a sleep sprite when the app
window loses focus (user is active elsewhere), and back to the normal face on
refocus, as a small bit of personality. This is the first of potentially
several face states, so the swap mechanism should be structured to add more
states later without rework.

## What Changes

- `MainWindow` tracks its own `Activated`/`Deactivated` events and pushes
  focus changes into `CalculatorView` via a new `SetFocused(bool)` method
  (the always-on-top pattern is reversed here since the direction of flow
  only ever goes window -> view), keeping `CalculatorView` window-chrome-agnostic.
- `CalculatorView` swaps the face `Image`'s `Source` between `face.png`
  (focused) and a new `face_sleep.png` (unfocused) through a small
  `FaceState` enum + `SetFaceState` method, structured so more states
  (beyond focused/unfocused) can be added later by extending the enum and a
  state-to-asset lookup, without touching the swap call sites.
- New art asset: `Assets/face_sleep.png` (real user-supplied sprite, not a
  placeholder).

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
- `app-shell`: `MainWindow` now also exposes window focus state to
  `CalculatorView`, alongside the existing always-on-top event.
- `calculator-layout`: the face display now has multiple states (focused,
  unfocused/sleep) instead of a single static image, swapped based on window
  focus.

## Impact

- `src/CuteGreenCalculator/MainWindow.xaml.cs` (Activated/Deactivated
  subscription, `WindowFocusChanged` event wiring).
- `src/CuteGreenCalculator/Controls/CalculatorView.xaml` /
  `CalculatorView.xaml.cs` (face `Image` gets a name, `FaceState` enum,
  `SetFaceState` method).
- New asset: `src/CuteGreenCalculator/Assets/face_sleep.png`.
- No changes to `CalculatorEngine` or button wiring.
