# button-styles Specification

## Purpose
TBD - created by archiving change add-pixel-button-styles. Update Purpose after archive.
## Requirements
### Requirement: Pixel button press states
Every calculator button SHALL show its normal art by default and swap to its
`_pressed` art variant while pressed, with no distinct visual change on mouse
hover.

#### Scenario: Pressing a button swaps its art
- **WHEN** the user presses and holds a calculator button
- **THEN** the button's image changes to that skin's `_pressed` art
- **AND** it reverts to the normal art on release

#### Scenario: Hovering does not change appearance
- **WHEN** the mouse moves over a calculator button without pressing it
- **THEN** the button's appearance is unchanged from its resting state

### Requirement: Embedded pixel font
Button labels and the display readout SHALL use a single embedded pixel-style
font rather than a system default font.

#### Scenario: Font renders as embedded, not a system fallback
- **WHEN** the application window is displayed
- **THEN** button labels and the display text render in the embedded pixel
  font

