## Why

The app shell currently renders only the calculator body background. To become a
usable calculator, it needs the actual button grid, status displays, and a pixel
font consistent with the art style, all laid out to match the reference mockup.

## What Changes

- Add a reusable pixel-button visual style per button skin (light_green, mid_green
  long/short, light_pink, mid_pink, dark_pink), swapping to the `_pressed` art on
  click with no hover state.
- Embed the "Press Start 2P" pixel font (OFL-licensed) and apply it to button
  labels and the display readout.
- Lay out the full calculator face: status row (face + heart displays), the main
  output screen, the 45/90/180 speed-dial buttons, and the 5-row numeric/operator
  grid, matching the reference mockup.

## Capabilities

### New Capabilities
- `button-styles`: Reusable pixel-art button visuals (normal/pressed art swap,
  sizing per skin, embedded pixel font) used by every clickable control.
- `calculator-layout`: The full visual arrangement of the calculator face —
  status displays, output screen, and button grid — matching the mockup.

### Modified Capabilities
- (none)

## Impact

- New files under `src/CuteGreenCalculator/Assets/Fonts/`,
  `src/CuteGreenCalculator/Styles/`.
- `CalculatorView.xaml` gains the full button/display layout (previously just the
  background image).
- No click behavior is wired to logic yet (that's `add-calculator-engine`); buttons
  only show correct visuals and pressed states.
