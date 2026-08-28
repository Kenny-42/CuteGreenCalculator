# app-shell Specification

## Purpose
TBD - created by archiving change add-wpf-app-shell. Update Purpose after archive.
## Requirements
### Requirement: WPF application shell
The application SHALL run as a WPF desktop app targeting `net10.0-windows`, with a
`MainWindow` that hosts calculator UI via a separate `CalculatorView` control rather
than defining calculator layout directly in the window. The window SHALL be
resizable, and the calculator face SHALL scale uniformly with the window rather
than staying pinned at a fixed pixel size.

#### Scenario: App launches and shows the calculator body
- **WHEN** the application is started
- **THEN** a window appears titled for the app, sized to fit the calculator body art
- **AND** the calculator background art is visible, fully opaque, with no layout gaps

#### Scenario: Window is resizable and the face scales with it
- **WHEN** the user drags the window's edge or corner to a new size
- **THEN** the window resizes
- **AND** the entire calculator face (background, buttons, screens) scales
  uniformly to fill the new window size, with no cropping and no distortion of
  the pixel art

#### Scenario: Aspect ratio is locked while resizing
- **WHEN** the user drags any window edge or corner
- **THEN** the resulting window width:height ratio stays equal to the calculator
  face's native 496:840 ratio, so the art is never stretched non-uniformly

#### Scenario: Window cannot be resized below a usable minimum
- **WHEN** the user attempts to shrink the window below its minimum size
- **THEN** the window stops shrinking at that minimum, keeping all controls
  legible and clickable

### Requirement: Crisp pixel art rendering
All bitmap art SHALL be rendered with nearest-neighbor scaling so upscaled pixel art
stays sharp rather than blurring.

#### Scenario: Background art is upscaled without blurring
- **WHEN** `background.png` is displayed at a size larger than its native pixel
  dimensions
- **THEN** its edges remain hard-pixeled (no anti-aliasing/blur artifacts introduced
  by the scaling)

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

