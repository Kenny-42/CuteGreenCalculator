# input-and-window-controls Specification

## Purpose
TBD - created by archiving change add-input-and-window-features. Update Purpose after archive.
## Requirements
### Requirement: Keyboard drives calculator entry
The application SHALL accept keyboard input as an alternative to clicking
buttons, routing each key through the same `CalculatorEngine` operations the
corresponding button uses. Typed characters SHALL be restricted to ones the
calculator understands (digits, `.`, `+ - * /`, and `@` for `√`); anything
else SHALL be silently rejected. The Delete key SHALL perform its ordinary
forward-delete/delete-selection role in the now-editable display rather than
acting as a `CE` shortcut; `CE` remains available via its button.

#### Scenario: Digits and decimal point
- **WHEN** the user types `1`, `2`, `.`, `5`
- **THEN** the display shows `12.5`, matching what clicking those buttons
  would produce

#### Scenario: Operators and equals
- **WHEN** the user types `5`, `+`, `3`, then presses Enter
- **THEN** the display shows `8`

#### Scenario: Escape clears
- **WHEN** the user has entered `5 + 3` and presses Escape
- **THEN** the display shows `0` and any pending operation is discarded,
  matching the `C` button

#### Scenario: Backspace deletes one character at the cursor
- **WHEN** the display shows `123` and the user presses Backspace
- **THEN** the display shows `12`

#### Scenario: Delete removes the character after the cursor
- **WHEN** the display shows `5+39`, the cursor is placed between `5+3` and
  `9`, and the user presses Delete
- **THEN** the display shows `5+3`

#### Scenario: F9 toggles sign, @ takes square root
- **WHEN** the user enters `5` and presses F9
- **THEN** the display shows `-5`
- **WHEN** the user then enters `9` and types `@`
- **THEN** the display shows `3`

#### Scenario: An unsupported character is ignored
- **WHEN** the display shows `5` and the user types a letter key
- **THEN** the display still shows `5`

### Requirement: Display value can be copied and pasted
The display SHALL be a real editable text field: the user can click into it,
move the cursor with the mouse or arrow keys, and select text with the mouse
or keyboard. The current value (or selection, if any) SHALL be copyable to
the system clipboard via Ctrl+C, a right-click context menu, or the copy
icon button. Pasted text SHALL be filtered to calculator-understood
characters and inserted at the cursor/selection, the same as typing it;
anything that doesn't survive filtering SHALL be dropped rather than
crashing the app or corrupting the display.

#### Scenario: Copy via Ctrl+C copies the selection
- **WHEN** the display shows `5+39` and the user selects `39` and presses
  Ctrl+C
- **THEN** the system clipboard contains `39`

#### Scenario: Copy with nothing selected copies the whole display
- **WHEN** the display shows `42` with no selection and the user presses
  Ctrl+C
- **THEN** the system clipboard contains `42`

#### Scenario: Paste inserts filtered text at the cursor
- **WHEN** the display shows `5+` with the cursor at the end and the system
  clipboard contains `3.5`
- **THEN** pressing Ctrl+V makes the display show `5+3.5`

#### Scenario: Paste invalid text is ignored
- **WHEN** the system clipboard contains `hello` and the user presses Ctrl+V
- **THEN** the display is unchanged and no error occurs

#### Scenario: Context menu copy and paste
- **WHEN** the user right-clicks the display and chooses "Copy" or "Paste"
- **THEN** the same behavior as the Ctrl+C / Ctrl+V shortcuts occurs

### Requirement: Always-on-top toggle
The UI SHALL provide a toggle button that sets or unsets the host window's
always-on-top state, with the button's own visual state (checked/unchecked)
indicating whether always-on-top is currently active. The toggle SHALL use
dedicated pin icon artwork (`star.png` unpinned / `star_pressed.png`
pinned), isolated in one style resource, rendered at the app's standard 8x
integer pixel scale.

#### Scenario: Enabling always-on-top
- **WHEN** the user clicks the always-on-top toggle while it is unchecked
- **THEN** the toggle becomes checked
- **AND** the host window's `Topmost` property becomes `true`

#### Scenario: Disabling always-on-top
- **WHEN** the user clicks the always-on-top toggle while it is checked
- **THEN** the toggle becomes unchecked
- **AND** the host window's `Topmost` property becomes `false`
