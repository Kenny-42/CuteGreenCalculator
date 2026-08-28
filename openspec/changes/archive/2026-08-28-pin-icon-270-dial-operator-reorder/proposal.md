## Why

Issue #19 asks for three UI polish items now that real art exists to
replace remaining placeholders: the always-on-top toggle still reuses
`heart.png`/`heart_screen.png` as a stand-in for dedicated pin artwork
(flagged as TEMPORARY since add-input-and-window-features), the speed-dial
row only has room for three buttons (45/90/180) so a fourth (270) can't be
added without new evenly-sized art, and the operator column order doesn't
match the desired reading of divide/multiply/subtract/add top-to-bottom.

## What Changes

- Swap `AlwaysOnTopToggleStyle`'s `heart.png`/`heart_screen.png` placeholder
  for dedicated `star.png` (unpinned) / `star_pressed.png` (pinned) art,
  rendered at the app's standard 8x integer scale (40x56, up from the old
  mismatched 40x40 box that distorted the heart's aspect ratio).
- Add a fourth speed-dial button (`Btn270`, wired identically to the other
  three: inserts "270" at the caret) using new `mid_green_button.png` /
  `mid_green_button_pressed.png` art sized the same as every other
  calculator button (96x80), replacing the old short/long speed-dial skins
  (`MidGreenShortButtonStyle` / `MidGreenLongButtonStyle`) so all four
  speed-dial buttons are uniform width and still align edge-to-edge with
  the output screen. **BREAKING** (internal only): removes
  `MidGreenShortButtonStyle`/`MidGreenLongButtonStyle` and their backing
  `mid_green_button_short*.png`/`mid_green_button_long*.png` assets, since
  nothing else references them.
- Reorder the rightmost operator column: move `BtnSqrt` from row 3 to the
  slot currently held by `BtnAdd` in row 7, bumping `BtnDivide`,
  `BtnMultiply`, `BtnSubtract`, and `BtnAdd` each up one row. Each button
  keeps its existing style/skin - this is a pure layout reorder, no new
  logic. Final column reads divide/multiply/subtract/add top-to-bottom
  (rows 3-6), with `=` remaining bottom-right on row 7 alongside the
  relocated `√`.
- New assets (`star.png`, `star_pressed.png`, `mid_green_button.png`,
  `mid_green_button_pressed.png`) are already copied into
  `src/CuteGreenCalculator/Assets/` and picked up by the existing
  `Assets\**\*.png` glob in the csproj - no project file changes needed.

## Capabilities

### New Capabilities
- (none)

### Modified Capabilities
- `input-and-window-controls`: the always-on-top toggle requirement no
  longer describes its art as temporary placeholder art.
- `calculator-layout`: the full-face layout requirement's description of
  the speed-dial row and the 5-row grid's button order both change.

Note: `button-styles` (press/hover behavior, embedded font) is unaffected -
the new/removed skins are new instances of the same existing requirement,
not a requirement change, so no delta spec is needed there.

## Impact

- Modified: `Styles/PixelButtonStyles.xaml` (new `MidGreenButtonStyle`,
  removed `MidGreenShortButtonStyle`/`MidGreenLongButtonStyle`, swapped
  `AlwaysOnTopToggleStyle` art + size).
- Modified: `Controls/CalculatorView.xaml` (speed-dial row gains `Btn270`
  and switches all four buttons to the new uniform style; rows 3-7 reorder
  `BtnSqrt`/`BtnDivide`/`BtnMultiply`/`BtnSubtract`/`BtnAdd`).
- Modified: `Controls/CalculatorView.xaml.cs` (wire `Btn270.Click`).
- Added: `Assets/star.png`, `Assets/star_pressed.png`,
  `Assets/mid_green_button.png`, `Assets/mid_green_button_pressed.png`.
- Removed: `Assets/mid_green_button_short.png`,
  `Assets/mid_green_button_short_pressed.png`,
  `Assets/mid_green_button_long.png`,
  `Assets/mid_green_button_long_pressed.png` (superseded, unreferenced
  after this change). `Assets/heart.png`/`Assets/heart_screen.png` are left
  in place since they're still used by the status-row heart display.
