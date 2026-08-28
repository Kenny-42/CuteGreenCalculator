# calculator-layout Specification

## Purpose
TBD - created by archiving change add-pixel-button-styles. Update Purpose after archive.
## Requirements
### Requirement: Full calculator face layout
The calculator face SHALL display, matching the reference mockup: a status row
(face display and heart display), the main output screen, three speed-dial
buttons (45, 90, 180), and a 5-row by 4-column grid covering `C CE +/- √`,
`7 8 9 ÷`, `4 5 6 ×`, `1 2 3 −`, and `0 . + =`. Every button row's leftmost and
rightmost button edges SHALL align with the output screen's left and right
edges, with even spacing between the columns within each row. The face display
SHALL support multiple visual states (at minimum: focused/awake and
unfocused/asleep), swapped by image source, structured so additional states
can be added later without reworking the swap mechanism.

#### Scenario: All controls are visible and correctly skinned
- **WHEN** the application window is displayed
- **THEN** every button listed above is visible with the correct button skin
  for its role (number vs. operator vs. function vs. equals vs. speed-dial)
- **AND** the status displays and output screen are visible above the button
  grid

#### Scenario: Button rows align with the output screen
- **WHEN** the application window is displayed at its native size
- **THEN** the leftmost button in every row (speed-dial row and the 5 grid
  rows) has its left edge aligned with the output screen's left edge
- **AND** the rightmost button in every row has its right edge aligned with
  the output screen's right edge
- **AND** the horizontal gaps between buttons within a row are equal

#### Scenario: Face shows the sleep sprite when the window is unfocused
- **WHEN** the application window loses focus (another window becomes active)
- **THEN** the face display swaps from the normal `face.png` sprite to the
  `face_sleep.png` sprite

#### Scenario: Face returns to normal when the window regains focus
- **WHEN** the application window regains focus after being unfocused
- **THEN** the face display immediately swaps back to the normal `face.png`
  sprite

