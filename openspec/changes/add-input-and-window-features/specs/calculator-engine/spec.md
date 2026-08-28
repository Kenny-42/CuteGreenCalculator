## ADDED Requirements

### Requirement: Backspace deletes the last entered character
The engine SHALL support removing the last character of the current entry.
If only one character (or a lone minus sign) remains, the entry SHALL reset
to `0`. Backspace has no effect while the engine is in an error state other
than the state itself.

#### Scenario: Backspace removes the last digit
- **WHEN** the display shows `123` and `Backspace` is invoked
- **THEN** the display shows `12`

#### Scenario: Backspace on a single digit resets to zero
- **WHEN** the display shows `7` and `Backspace` is invoked
- **THEN** the display shows `0`

#### Scenario: Backspace never leaves a bare minus sign
- **WHEN** the display shows `-5`, `Backspace` is invoked, then invoked again
- **THEN** the display shows `0` after the second invocation, never `-`

### Requirement: Pasted values are validated before acceptance
The engine SHALL support accepting an external numeric string as the new
current entry. Text that does not parse as a finite number SHALL be
ignored, leaving the display unchanged.

#### Scenario: Valid pasted number replaces the current entry
- **WHEN** the display shows `0` and `PasteValue("3.5")` is invoked
- **THEN** the display shows `3.5`

#### Scenario: Invalid pasted text is ignored
- **WHEN** the display shows `42` and `PasteValue("hello")` is invoked
- **THEN** the display still shows `42`
