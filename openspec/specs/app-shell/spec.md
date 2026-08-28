# app-shell Specification

## Purpose
TBD - created by archiving change add-wpf-app-shell. Update Purpose after archive.
## Requirements
### Requirement: WPF application shell
The application SHALL run as a WPF desktop app targeting `net10.0-windows`, with a
borderless `MainWindow` (`WindowStyle="None"` with `WindowChrome` for
edge/corner resize) that hosts a custom title bar strip above the calculator
face, both via separate `TitleBarView` and `CalculatorView` controls rather
than defining any of this layout directly in the window. The window SHALL be
resizable, and the title bar + calculator face SHALL scale together as one
unit uniformly with the window rather than staying pinned at a fixed pixel
size.

#### Scenario: App launches and shows the calculator body
- **WHEN** the application is started
- **THEN** a window appears with no native title bar, sized to fit the title
  bar strip plus the calculator body art
- **AND** the calculator background art is visible, fully opaque, with no layout gaps

#### Scenario: Window is resizable and the face scales with it
- **WHEN** the user drags the window's edge or corner to a new size
- **THEN** the window resizes
- **AND** the title bar strip and the entire calculator face (background,
  buttons, screens) scale together uniformly to fill the new window size,
  with no cropping and no distortion of the pixel art

#### Scenario: Aspect ratio is locked while resizing
- **WHEN** the user drags any window edge or corner
- **THEN** the resulting window width:height ratio stays equal to the
  combined title bar + calculator face's native 496:896 ratio, so the art is
  never stretched non-uniformly

#### Scenario: Window cannot be resized below a usable minimum
- **WHEN** the user attempts to shrink the window below its minimum size
- **THEN** the window stops shrinking at that minimum, keeping all controls
  legible and clickable

#### Scenario: Maximizing lets the face letterbox rather than distort
- **WHEN** the user maximizes the window (via the title bar's
  maximize button, double-clicking the title bar, or a Windows Snap gesture)
- **THEN** the window fills the screen
- **AND** the title bar + calculator face scale uniformly within it,
  letterboxed against the window background rather than stretched
  non-uniformly

### Requirement: Crisp pixel art rendering
All bitmap art SHALL be rendered with nearest-neighbor scaling so upscaled pixel art
stays sharp rather than blurring.

#### Scenario: Background art is upscaled without blurring
- **WHEN** `background.png` is displayed at a size larger than its native pixel
  dimensions
- **THEN** its edges remain hard-pixeled (no anti-aliasing/blur artifacts introduced
  by the scaling)

### Requirement: Window/view separation
Window-level chrome concerns (title, icon, sizing, topmost, focus state,
minimize/maximize/close, drag-to-move) SHALL live in `MainWindow` and the new
`TitleBarView`, while all calculator face layout and visuals SHALL live in
`CalculatorView`. `MainWindow` SHALL expose its focus state to
`CalculatorView` without `CalculatorView` referencing `MainWindow` or
window-chrome APIs directly, following the same pattern as the always-on-top
toggle (`CalculatorView` raises/receives plain events or method calls, never
touches `Window` itself). `TitleBarView`, unlike `CalculatorView`, is
window-chrome-aware by design and may act on the hosting `Window` directly
(minimize/maximize/restore/close/drag), since driving window state is its
sole purpose.

#### Scenario: CalculatorView has no window-chrome dependency
- **WHEN** `CalculatorView` is inspected
- **THEN** it does not reference `MainWindow`-specific chrome (e.g. it does not set
  `WindowStyle`, `ResizeMode`, or the window `Title` itself)

#### Scenario: CalculatorView reacts to window focus without knowing about Window
- **WHEN** `MainWindow` becomes active or inactive
- **THEN** `CalculatorView` is informed via a plain method call/event, not by
  `CalculatorView` subscribing to `Window.Activated`/`Deactivated` itself

#### Scenario: The logo button resets the calculator via an event, not direct coupling
- **WHEN** the title bar's logo button is clicked
- **THEN** `TitleBarView` raises a plain event that `MainWindow` wires to
  `CalculatorView`'s reset method, rather than `TitleBarView` referencing
  `CalculatorView` directly

