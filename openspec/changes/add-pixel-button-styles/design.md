## Context

Six button skins exist as PNG pairs (normal + pressed), all natively small
(10-22 x 10 px) meant to be scaled up 8x alongside the background (which is
496x840 at 8x, per `add-wpf-app-shell`). No hover state exists in the art, so
buttons must not visually react to mouse-over, only to press.

## Goals / Non-Goals

**Goals:**
- One `ControlTemplate` shape reused via six named `Style` resources (one per
  skin), each baking in that skin's fixed Width/Height (native size x8) so call
  sites just set `Style` and `Content`.
- A single embedded pixel font used consistently for every label and the readout.
- Layout close to the mockup; exact pixel alignment is deferred to the polish
  change.

**Non-Goals:**
- Wiring buttons to calculator logic (next change).
- Designing new art for anything not already covered by existing assets.

## Decisions

- **Font**: "Press Start 2P" (SIL Open Font License), downloaded from the
  Google Fonts repository and committed under
  `Assets/Fonts/PressStart2P-Regular.ttf` (+ its `OFL.txt` license file).
  Referenced in XAML via
  `pack://application:,,,/CuteGreenCalculator;component/Assets/Fonts/#Press Start 2P`.
  Used for both button labels and the display readout, avoiding a second font
  dependency. This can be swapped for a different pixel/LCD font later by
  changing one resource.
- **Style-per-skin sizing**: Each of the 6 styles hardcodes its `Width`/`Height`
  (native px * 8) since skin and size are 1:1 in this art set:
  - `LightGreenButtonStyle`, `LightPinkButtonStyle`, `MidPinkButtonStyle`,
    `DarkPinkButtonStyle`: 96x80
  - `MidGreenShortButtonStyle`: 112x80
  - `MidGreenLongButtonStyle`: 176x80
- **Template mechanics**: `Grid` with an `Image` (normal art, NearestNeighbor
  scaling) behind a `ContentPresenter`; an `IsPressed` trigger swaps the
  `Image.Source` to the `_pressed` art. No `IsMouseOver` trigger exists, so
  there is no hover visual by construction. `FocusVisualStyle` is disabled and
  default `Button` chrome (background/border) is stripped via the template.
- **Speed-dial button widths**: 45 and 90 use `MidGreenShortButtonStyle` (2
  digits fit); 180 uses `MidGreenLongButtonStyle` (3 digits). This uses both
  green preset assets instead of leaving one unused.
- **Layout container**: A single vertical `StackPanel`, centered in the 496x840
  background, containing: status row (face_screen + heart_screen), output
  screen, speed-dial row, then 5 rows of 4 buttons each. Rows use horizontal
  `StackPanel`s with fixed gaps so spacing is easy to tune later.
- **Display readout**: A `TextBlock` (`x:Name="DisplayText"`) overlaid on
  `output_screen.png`, right-aligned, using the same pixel font. Shows a static
  placeholder ("0") in this change; wiring to live calculator state happens in
  `add-calculator-engine`.

## Risks / Trade-offs

- Press Start 2P's fixed block-letter shapes are wide; button font size is kept
  modest (20px at 8x scale) to avoid clipping "+/-" and "CE" labels inside
  96px-wide buttons. Verified visually via screenshot before merging.
- **DPI awareness**: added an explicit `app.manifest` declaring Per-Monitor-V2
  DPI awareness (`ApplicationManifest` in the csproj). Without it, Windows can
  run the app DPI-unaware and let the OS bitmap-stretch its rendered output on
  scaled displays, which would blur/distort the nearest-neighbor-scaled pixel
  art this whole app depends on. (This was also discovered to be the cause of
  a very confusing local screenshot-tooling artifact during development: a
  DPI-unaware *screenshot* helper undercaptured the DPI-aware app's window,
  making content look like it was silently missing. Not an app bug - just a
  reminder that any future dev tooling capturing this app's window should
  itself be Per-Monitor-V2 aware.)
