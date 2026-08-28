## MODIFIED Requirements

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
