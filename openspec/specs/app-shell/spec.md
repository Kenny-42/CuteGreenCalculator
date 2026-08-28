# app-shell Specification

## Purpose
TBD - created by archiving change add-wpf-app-shell. Update Purpose after archive.
## Requirements
### Requirement: WPF application shell
The application SHALL run as a WPF desktop app targeting `net10.0-windows`, with a
`MainWindow` that hosts calculator UI via a separate `CalculatorView` control rather
than defining calculator layout directly in the window.

#### Scenario: App launches and shows the calculator body
- **WHEN** the application is started
- **THEN** a window appears titled for the app, sized to fit the calculator body art
- **AND** the calculator background art is visible, fully opaque, with no layout gaps

### Requirement: Crisp pixel art rendering
All bitmap art SHALL be rendered with nearest-neighbor scaling so upscaled pixel art
stays sharp rather than blurring.

#### Scenario: Background art is upscaled without blurring
- **WHEN** `background.png` is displayed at a size larger than its native pixel
  dimensions
- **THEN** its edges remain hard-pixeled (no anti-aliasing/blur artifacts introduced
  by the scaling)

### Requirement: Window/view separation
Window-level chrome concerns (title, icon, sizing, topmost) SHALL live in
`MainWindow`, while all calculator face layout and visuals SHALL live in
`CalculatorView`, so the window chrome can be replaced later without modifying
calculator layout or logic.

#### Scenario: CalculatorView has no window-chrome dependency
- **WHEN** `CalculatorView` is inspected
- **THEN** it does not reference `MainWindow`-specific chrome (e.g. it does not set
  `WindowStyle`, `ResizeMode`, or the window `Title` itself)

