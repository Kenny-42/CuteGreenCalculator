# calculator-engine Specification

## Purpose
TBD - created by archiving change add-calculator-engine. Update Purpose after archive.
## Requirements
### Requirement: Basic arithmetic entry and evaluation
The engine SHALL support entering digits and a decimal point, applying a
chained binary operator (+, −, ×, ÷), and evaluating with `=`.

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

### Requirement: Clear semantics
`C` SHALL fully reset the engine to its initial state. `CE` SHALL reset only
the value currently being entered, preserving any in-progress chained
operation.

#### Scenario: C fully resets
- **WHEN** the user enters `5`, presses `+`, enters `3`, then presses `C`
- **THEN** the display shows `0`
- **AND** pressing `7` then `=` shows `7` (no leftover pending operation)

#### Scenario: CE preserves a pending operation
- **WHEN** the user enters `5`, presses `+`, enters `39`, presses `CE`, enters
  `3`, then presses `=`
- **THEN** the display shows `8`

### Requirement: Unary operations
`+/-` SHALL toggle the sign of the currently displayed value. `√` SHALL
replace the currently displayed value with its square root.

#### Scenario: Sign toggle
- **WHEN** the user enters `5` then presses `+/-`
- **THEN** the display shows `-5`

#### Scenario: Square root
- **WHEN** the user enters `9` then presses `√`
- **THEN** the display shows `3`

### Requirement: Error handling
Dividing by zero or taking the square root of a negative number SHALL put the
engine into an error state where the display shows `Error` and only `C` is
accepted as further input.

#### Scenario: Divide by zero
- **WHEN** the user enters `5`, presses `÷`, enters `0`, then presses `=`
- **THEN** the display shows `Error`
- **AND** pressing any digit or operator has no effect
- **AND** pressing `C` returns the display to `0` and accepts further input

#### Scenario: Square root of a negative number
- **WHEN** the user enters `4`, presses `+/-` (making it `-4`), then presses `√`
- **THEN** the display shows `Error`

### Requirement: Speed-dial digit shortcuts
The UI SHALL provide 45, 90, and 180 speed-dial buttons that feed their
label's digits through the engine's digit-entry path one character at a
time, producing the same result as the user pressing those digits
individually.

#### Scenario: Speed-dial on a fresh entry
- **WHEN** the display shows `0` (a fresh entry) and the user presses `45`
- **THEN** the display shows `45`

#### Scenario: Speed-dial mid-entry appends
- **WHEN** the user has entered `7` (not yet a fresh entry) and presses `90`
- **THEN** the display shows `790`

#### Scenario: Speed-dial right after equals replaces the entry
- **WHEN** the user computes `2 + 3 =` (showing `5`) and then presses `180`
- **THEN** the display shows `180`

