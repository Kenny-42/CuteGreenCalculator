## MODIFIED Requirements

### Requirement: Full calculator face layout
The calculator face SHALL display, matching the reference mockup: a status
row (face display and heart display), the main output screen, four
speed-dial buttons (45, 90, 180, 270) of uniform size, and a 5-row by
4-column grid covering `C CE +/-`, `7 8 9 ÷`, `4 5 6 ×`, `1 2 3 −`, and
`0 . √ =`. Every button row's leftmost and rightmost button edges SHALL
align with the output screen's left and right edges, with even spacing
between the columns within each row. The face display SHALL support
multiple visual states (at minimum: focused/awake and unfocused/asleep),
swapped by image source, structured so additional states can be added
later without reworking the swap mechanism.

The output screen SHALL host an editable text field showing the calculator's
expression, rather than a static label, and its font size SHALL shrink as
the expression grows so it is never clipped or truncated. A copy icon button
SHALL sit in the output screen's top-left corner, performing the same copy
action as Ctrl+C or the display's right-click "Copy" menu item.

The heart display SHALL be a group of 4 independently-clickable toggles
forming a single left-to-right threshold: clicking a heart toggles it and
every heart to its right on, and untoggles every heart to its left. Clicking
the leftmost currently-toggled heart again untoggles the whole group back to
all-off.

#### Scenario: All controls are visible and correctly skinned
- **WHEN** the application window is displayed
- **THEN** every button listed above is visible with the correct button skin
  for its role (number vs. operator vs. function vs. equals vs. speed-dial)
- **AND** the status displays and output screen are visible above the button
  grid
- **AND** the copy icon button is visible in the output screen's top-left
  corner

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

#### Scenario: A long expression shrinks to fit
- **WHEN** the expression grows too wide to fit the output screen at the
  default font size
- **THEN** the display's font size decreases until the text fits, down to a
  minimum readable size

#### Scenario: The copy button copies the display
- **WHEN** the user clicks the copy icon button
- **THEN** the selected text is copied to the clipboard, or the whole
  display value if nothing is selected

#### Scenario: Clicking a heart toggles it and everything to its right
- **WHEN** the user clicks a heart that is not the leftmost currently-toggled
  heart
- **THEN** that heart and every heart to its right switch to the pressed
  (`heart_pressed.png`) state
- **AND** every heart to its left switches to the normal (`heart.png`) state

#### Scenario: Clicking the leftmost toggled heart clears the group
- **WHEN** the user clicks the heart that is currently the leftmost toggled
  heart
- **THEN** all 4 hearts switch to the normal state
