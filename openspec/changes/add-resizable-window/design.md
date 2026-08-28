## Context

`MainWindow` is currently `SizeToContent="WidthAndHeight"` with `ResizeMode="CanMinimize"`, hosting `CalculatorView`'s fixed `Grid Width="496" Height="840"` (the background art at native 62x105 rendered at an 8x integer scale). Button rows inside that grid are `StackPanel`s of fixed-width buttons (96px for standard rows, 96-176px for the speed-dial row) separated by 8px margins, all `HorizontalAlignment="Center"` within the 496-wide grid. The output screen above them is 432px wide, also centered. Because the button rows (408px / 416px) are narrower than the output screen (432px) and both are independently centered, their edges don't line up — the button block sits a few pixels inset from the screen's edges on both sides.

## Goals / Non-Goals

**Goals:**
- Window can be resized by dragging edges/corners, with the whole calculator face scaling uniformly (no distortion, no cropping).
- Aspect ratio locked to the face's native 496:840 ratio so users can't stretch it into a non-square-pixel shape.
- Button row edges align with the output screen's edges; column gaps are even within each row.

**Non-Goals:**
- No new art assets or button re-skinning.
- No custom/borderless title bar (tracked separately, explicitly sequenced after this change).
- No maximize/fullscreen-specific layout handling beyond what uniform scaling already provides.

## Decisions

- **Viewbox for scaling**: Wrap the existing 496x840 `Grid` in `CalculatorView.xaml` with a `<Viewbox Stretch="Uniform">`. This keeps all existing layout, styles, and pixel-perfect nearest-neighbor image rendering untouched (the Grid still lays out at its native 496x840 "design" size; the Viewbox scales the rendered result as one unit). Alternative considered: making every element's Width/Height relative (Grid *-star columns, no fixed sizes) — rejected as a much larger, riskier rewrite of `CalculatorView.xaml` for no behavioral benefit over a Viewbox.
- **Aspect ratio lock via WM_SIZING**: Rather than relying on the Viewbox alone (which would letterbox/pillarbox if the window itself were resized to an off-ratio shape, leaving visible background-color bars), `MainWindow.xaml.cs` hooks `HwndSource` and intercepts `WM_SIZING` to adjust the proposed resize rectangle so width:height always stays 496:840, based on which edge/corner is being dragged. This keeps the window itself always exactly the right shape, so the Viewbox never has to letterbox. Alternative considered: do nothing beyond the Viewbox and accept letterbox bars — rejected because the requirement explicitly asks the art to never appear stretched/cropped *and* implies the window itself should feel intentional at any size, not show dead bars.
- **MinWidth/MinHeight**: Set to half the native size (248x420) — small enough to be a real "resizable" window, large enough that buttons stay legible/clickable. `MaxWidth`/`MaxHeight` are left unset (unbounded, matching a normal resizable window).
- **Button row alignment fix**: Increase inter-column margins from 8px to 16px on every button row (standard 4-column rows: 4×96 + 3×16 = 432px; speed-dial row: 112+112+176 + 2×16 = 432px). This makes every row exactly 432px wide — the same as the output screen — so with both still `HorizontalAlignment="Center"` in the same parent, all edges line up automatically. No X/Y offset hacks needed. Alternative considered: explicitly setting `Margin`/`HorizontalAlignment="Stretch"` with a `Grid` of star columns per row — rejected as a bigger structural change for the same visual result.

## Risks / Trade-offs

- [Risk] `WM_SIZING` interception is Win32 interop (`PresentationFramework`/`HwndSource.AddHook`), which is more fragile than pure XAML → Mitigation: isolate it in a small, well-commented method in `MainWindow.xaml.cs`; verify manually by dragging each edge/corner and confirming the ratio holds.
- [Risk] Changing standard-row margins from 8px→16px is a visual change beyond pure alignment → Mitigation: this is required by the math to hit 432px consistently across all rows; verify against the mockup screenshot that it still looks intentional (it increases spacing slightly, doesn't change button sizes).
- [Risk] `MinWidth`/`MinHeight` chosen without a hard design spec → Mitigation: pick a reasonable floor (248x420, half native size) and note it's adjustable later.

## Migration Plan

Single PR, no data/state migration. Manual verification: launch the app, drag-resize from each edge and corner and confirm the face scales without distortion and never leaves visible letterbox bars; screenshot at native and at a resized size and compare button/screen edge alignment.
