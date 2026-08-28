## ADDED Requirements

### Requirement: Custom pixel-art title bar
The application SHALL show a custom pixel-art title bar strip above the
calculator face instead of the native Windows title bar. The strip SHALL
display, left to right: a logo/daisy button, the app's title text, and then
minimize, maximize/restore, and close buttons, each skinned with dedicated
normal and pressed art matching the rest of the app's button styling.

#### Scenario: Title bar renders with no native chrome
- **WHEN** the application window is displayed
- **THEN** no native Windows title bar, icon-menu, or border is visible
- **AND** the custom title bar strip is visible above the calculator face,
  showing the logo button, the app title text, and the minimize,
  maximize/restore, and close buttons

#### Scenario: Buttons show a pressed state
- **WHEN** the user presses and holds any title bar button
- **THEN** that button's art swaps to its pressed variant while held, and
  reverts on release

### Requirement: Drag-to-move
Dragging the title bar strip, on any part not covered by a button, SHALL
move the window, matching standard Windows title bar behavior.

#### Scenario: Dragging the strip moves the window
- **WHEN** the user presses the left mouse button on the title bar strip
  (outside its buttons) and drags
- **THEN** the window follows the cursor
- **AND** the title bar strip's background shows its pressed art while the
  button is held down

### Requirement: Standard resize at edges and corners
Even though the window is borderless, dragging at the window's outer edges
or corners SHALL resize it, same as a normal window, and this resize SHALL
remain constrained to the calculator face's locked aspect ratio.

#### Scenario: Edge/corner drag resizes the borderless window
- **WHEN** the user drags at the outer edge or corner of the window
- **THEN** the window resizes, following the same aspect-ratio lock as
  described in the `app-shell` capability

### Requirement: Double-click toggles maximize/restore
Double-clicking the title bar strip SHALL toggle the window between
maximized and its previous restored size/position, matching standard
Windows behavior.

#### Scenario: Double-click maximizes a restored window
- **WHEN** the user double-clicks the title bar strip while the window is
  not maximized
- **THEN** the window maximizes

#### Scenario: Double-click restores a maximized window
- **WHEN** the user double-clicks the title bar strip while the window is
  maximized
- **THEN** the window returns to its prior restored size and position

### Requirement: Window control buttons
The minimize, maximize/restore, and close buttons SHALL perform the
equivalent native window action. The maximize/restore button's art SHALL
reflect the window's current state, regardless of how that state changed.

#### Scenario: Minimize button minimizes the window
- **WHEN** the user clicks the minimize button
- **THEN** the window minimizes to the taskbar

#### Scenario: Maximize button maximizes the window
- **WHEN** the user clicks the maximize/restore button while the window is
  not maximized
- **THEN** the window maximizes
- **AND** the button's art swaps to the restore icon

#### Scenario: Restore button restores the window
- **WHEN** the user clicks the maximize/restore button while the window is
  maximized
- **THEN** the window returns to its prior restored size and position
- **AND** the button's art swaps back to the maximize icon

#### Scenario: Close button closes the window
- **WHEN** the user clicks the close button
- **THEN** the application window closes

#### Scenario: Maximize/restore icon stays correct after an external state change
- **WHEN** the window's maximized state changes by any means other than the
  maximize/restore button (e.g. double-clicking the title bar, a Windows
  Snap gesture)
- **THEN** the maximize/restore button's art updates to match the new state

### Requirement: Logo button resets the calculator
Clicking the logo/daisy button SHALL reset the calculator display, identical
in effect to clicking the `C` button.

#### Scenario: Logo button clears the display
- **WHEN** the user clicks the logo/daisy button
- **THEN** the calculator display resets exactly as if `C` had been clicked
