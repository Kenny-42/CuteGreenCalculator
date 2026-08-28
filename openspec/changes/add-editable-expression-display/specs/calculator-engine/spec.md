## MODIFIED Requirements

### Requirement: Basic arithmetic entry and evaluation

The engine SHALL support entering digits and a decimal point, applying a
chained binary operator (+, −, ×, ÷), and evaluating with `=`. The display
SHALL show the whole expression as typed (e.g. `12+34`), not only the
operand currently being entered.

#### Scenario: Simple addition
- **WHEN** the user enters `12`, presses `+`, enters `8`, then presses `=`
- **THEN** the display shows `20`

#### Scenario: Chained operations evaluate left to right immediately
- **WHEN** the user enters `5`, presses `+`, enters `3`, presses `+`, enters
  `2`, then presses `=`
- **THEN** the display shows `10`

#### Scenario: Repeating equals repeats the last operation
- **WHEN** the user computes `4 + 6 =` (showing `10`) and presses `=` again
- **THEN** the display shows `16`

#### Scenario: The full expression is visible while typing
- **WHEN** the user enters `5`, presses `+`, then enters `3`
- **THEN** the display shows `5+3` (not just `3`)

### Requirement: Unary operations

`+/-` SHALL toggle the sign of the trailing operand (the number after the
last operator, or the whole expression if there is no operator), in place.
`√` SHALL replace the trailing operand with its square root, in place. This
lets both operations be used mid-chain without disturbing earlier operands.

#### Scenario: Sign toggle
- **WHEN** the user enters `5` then presses `+/-`
- **THEN** the display shows `-5`

#### Scenario: Square root
- **WHEN** the user enters `9` then presses `√`
- **THEN** the display shows `3`

#### Scenario: Sign toggle applies only to the trailing operand
- **WHEN** the user enters `5`, presses `+`, enters `3`, then presses `+/-`
- **THEN** the display shows `5+-3`

#### Scenario: Square root applies only to the trailing operand
- **WHEN** the user enters `5`, presses `+`, enters `9`, then presses `√`
- **THEN** the display shows `5+3`

### Requirement: Clear semantics

`C` SHALL fully reset the engine to its initial state. `CE` SHALL remove
only the trailing operand being entered, preserving any earlier chained
operation (e.g. `5+39` becomes `5+`).

#### Scenario: C fully resets
- **WHEN** the user enters `5`, presses `+`, enters `3`, then presses `C`
- **THEN** the display shows `0`
- **AND** pressing `7` then `=` shows `7` (no leftover pending operation)

#### Scenario: CE preserves a pending operation
- **WHEN** the user enters `5`, presses `+`, enters `39`, presses `CE`, enters
  `3`, then presses `=`
- **THEN** the display shows `8`

## ADDED Requirements

### Requirement: Caret-aware mid-expression editing

The engine SHALL support inserting text at an arbitrary position within the
expression (replacing a selection first, if any), used uniformly by typed
characters, pasted text, and button clicks. An insertion SHALL be rejected,
leaving the expression unchanged, if it is not a legal in-progress
expression: unsupported characters, a second decimal point within one
number, or an operator with no operand before it (a `-` is additionally
accepted as a leading sign at the start of a number). The engine SHALL also
support resyncing its state from text an edit already produced elsewhere
(e.g. a native Backspace/Delete/Cut in the display), rejecting the resync
under the same rules.

#### Scenario: Inserting a digit mid-string
- **WHEN** the expression is `13`, the cursor is placed between `1` and `3`,
  and the user types `2`
- **THEN** the display shows `123`

#### Scenario: Replacing a selection
- **WHEN** the expression is `5+39`, the `39` is selected, and the user
  types `7`
- **THEN** the display shows `5+7`

#### Scenario: A second operator in a row is rejected
- **WHEN** the expression is `5+` and the user types `*`
- **THEN** the display still shows `5+`

#### Scenario: A second decimal point in one number is rejected
- **WHEN** the expression is `1.2` and the user places the cursor after the
  `1` and types `.`
- **THEN** the display still shows `1.2`

#### Scenario: Unsupported characters are filtered out, even on paste
- **WHEN** the user pastes `ab5cd`
- **THEN** only `5` is inserted

#### Scenario: A fresh entry is replaced by a new number, not appended to
- **WHEN** the user computes `2 + 3 =` (showing `5`) and then types `1`
- **THEN** the display shows `1`, not `51`
- **AND** typing `8` and `0` next appends normally, showing `180`
