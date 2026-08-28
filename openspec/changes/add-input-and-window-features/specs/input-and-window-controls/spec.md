## ADDED Requirements

### Requirement: Keyboard drives calculator entry
The application SHALL accept keyboard input as an alternative to clicking
buttons, routing each key through the same `CalculatorEngine` operations the
corresponding button uses.

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

#### Scenario: Delete clears the current entry
- **WHEN** the user has entered `5`, pressed `+`, entered `39`, then presses
  Delete
- **THEN** the display shows `0` and the pending `+` operation is preserved,
  matching the `CE` button

#### Scenario: Backspace deletes one character
- **WHEN** the display shows `123` and the user presses Backspace
- **THEN** the display shows `12`

#### Scenario: F9 toggles sign, @ takes square root
- **WHEN** the user enters `5` and presses F9
- **THEN** the display shows `-5`
- **WHEN** the user then enters `9` and types `@`
- **THEN** the display shows `3`

### Requirement: Display value can be copied and pasted
The display's current value SHALL be copyable to the system clipboard and
pasteable back in, via both keyboard shortcuts and a right-click context
menu. Pasted text SHALL be validated before being accepted; text that isn't
a valid number SHALL be ignored rather than crashing the app or corrupting
the display.

#### Scenario: Copy via Ctrl+C
- **WHEN** the display shows `42` and the user presses Ctrl+C
- **THEN** the system clipboard contains `42`

#### Scenario: Paste a valid number via Ctrl+V
- **WHEN** the system clipboard contains `3.5` and the user presses Ctrl+V
- **THEN** the display shows `3.5` and further digit entry continues from it

#### Scenario: Paste invalid text is ignored
- **WHEN** the system clipboard contains `hello` and the user presses Ctrl+V
- **THEN** the display is unchanged and no error occurs

#### Scenario: Context menu copy and paste
- **WHEN** the user right-clicks the display and chooses "Copy" or "Paste"
- **THEN** the same behavior as the Ctrl+C / Ctrl+V shortcuts occurs

### Requirement: Always-on-top toggle
The UI SHALL provide a toggle button that sets or unsets the host window's
always-on-top state, with the button's own visual state (checked/unchecked)
indicating whether always-on-top is currently active. The toggle's artwork
is temporary placeholder art (`heart.png` / `heart_screen.png`), isolated in
one style resource so it can be swapped for permanent art without touching
any other code.

#### Scenario: Enabling always-on-top
- **WHEN** the user clicks the always-on-top toggle while it is unchecked
- **THEN** the toggle becomes checked
- **AND** the host window's `Topmost` property becomes `true`

#### Scenario: Disabling always-on-top
- **WHEN** the user clicks the always-on-top toggle while it is checked
- **THEN** the toggle becomes unchecked
- **AND** the host window's `Topmost` property becomes `false`
