## ADDED Requirements

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
