## MODIFIED Requirements

### Requirement: Window/view separation
Window-level chrome concerns (title, icon, sizing, topmost, focus state)
SHALL live in `MainWindow`, while all calculator face layout and visuals
SHALL live in `CalculatorView`, so the window chrome can be replaced later
without modifying calculator layout or logic. `MainWindow` SHALL expose its
focus state to `CalculatorView` without `CalculatorView` referencing
`MainWindow` or window-chrome APIs directly, following the same pattern as
the always-on-top toggle (`CalculatorView` raises/receives plain events or
method calls, never touches `Window` itself).

#### Scenario: CalculatorView has no window-chrome dependency
- **WHEN** `CalculatorView` is inspected
- **THEN** it does not reference `MainWindow`-specific chrome (e.g. it does not set
  `WindowStyle`, `ResizeMode`, or the window `Title` itself)

#### Scenario: CalculatorView reacts to window focus without knowing about Window
- **WHEN** `MainWindow` becomes active or inactive
- **THEN** `CalculatorView` is informed via a plain method call/event, not by
  `CalculatorView` subscribing to `Window.Activated`/`Deactivated` itself
