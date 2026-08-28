# calculator-layout Specification

## Purpose
TBD - created by archiving change add-pixel-button-styles. Update Purpose after archive.
## Requirements
### Requirement: Full calculator face layout
The calculator face SHALL display, matching the reference mockup: a status row
(face display and heart display), the main output screen, three speed-dial
buttons (45, 90, 180), and a 5-row by 4-column grid covering `C CE +/- √`,
`7 8 9 ÷`, `4 5 6 ×`, `1 2 3 −`, and `0 . + =`.

#### Scenario: All controls are visible and correctly skinned
- **WHEN** the application window is displayed
- **THEN** every button listed above is visible with the correct button skin
  for its role (number vs. operator vs. function vs. equals vs. speed-dial)
- **AND** the status displays and output screen are visible above the button
  grid

