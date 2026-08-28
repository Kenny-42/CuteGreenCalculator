## Context

`MainWindow` currently uses the native Win32 title bar; `CalculatorView` owns
a `Viewbox` that scales its fixed 496x840 design grid to fill the window
(issue #15). The new title bar strip is native 62x7 px (same width as
`background.png`'s 62px), so at the app's established 8x integer scale it
renders as 496x56 - exactly as wide as the calculator face.

## Decision: MainWindow owns the single outer Viewbox

Rather than nesting a second `Viewbox` inside a new `TitleBarView` alongside
`CalculatorView`'s existing one, `CalculatorView`'s internal `Viewbox` is
removed and `MainWindow` gets one `Viewbox` wrapping a 496x896 grid with two
rows: the new `TitleBarView` (56px) and the unchanged `CalculatorView`
content (840px, still laid out as its own 496x840 design grid, just no
longer scaled by itself). This keeps exactly one scaling boundary for the
whole face+chrome unit, matches the project's established "design pixels,
then one uniform scale" pattern, and keeps `CalculatorView` exactly as
window-chrome-agnostic as it already was - it just stops being responsible
for its own scaling.

**Alternative considered**: give `TitleBarView` its own `Viewbox` and keep
`CalculatorView`'s. Rejected - two independent `Viewbox`es scaling
separately could round to slightly different pixel scales at odd window
sizes, causing a visible seam between the title bar and the face; one shared
`Viewbox` guarantees they always scale identically.

## Decision: WindowChrome with CaptionHeight=0, manual drag

`WindowStyle="None"` alone drops the native frame but also drops edge/corner
resize entirely (no border left to grab). `WindowChrome`
(`System.Windows.Shell`) is added with a nonzero `ResizeBorderThickness` to
restore invisible resize grab-zones on all edges/corners - the existing
`WM_SIZING` hook (issue #15) keeps working unchanged since `WindowChrome`
still lets that message through during an interactive drag.

`CaptionHeight` is set to `0` rather than some positive value, because a
positive `CaptionHeight` would make WPF treat that whole strip as an
automatic system caption (auto-drag, but also auto-double-click-maximize
*and* it would swallow clicks meant for the logo/min/max/close buttons
sitting inside it, requiring `WindowChrome.IsHitTestVisibleInChrome="True"`
per button as a workaround). Instead, `TitleBarView` handles drag and
double-click itself via `Window.DragMove()` on a plain `MouseLeftButtonDown`
handler on the strip's non-button area - simpler to reason about, and it's
already how this app's buttons are built to consume their own clicks.

**Trade-off accepted**: without `CaptionHeight`, Windows' Aero-Snap-on-drag
(dragging the title bar to a screen edge to auto-tile) doesn't trigger
automatically. This isn't in issue #16's requirements list, so it's left for
a future issue if wanted.

## Decision: Maximize lets the aspect ratio letterbox instead of locking it

The `WM_SIZING` aspect-ratio lock only fires during an interactive
edge/corner drag - `WindowState.Maximized` is a separate code path that
doesn't send `WM_SIZING`, so maximizing fills the whole monitor (standard
behavior) rather than snapping to the nearest 496:896-ratio rectangle. The
outer `Viewbox` (`Stretch="Uniform"`) then letterboxes the calculator
face+chrome inside that maximized window rather than distorting it -
`Window.Background` is set to the background art's own border color
(`#7B8A5E`) so the letterbox bars blend in rather than showing bare white.

## Decision: Maximize/restore is a single button whose art swaps

Rather than two buttons shown/hidden based on state (extra Grid bookkeeping,
and a flash if state changes mid-click), `TitleBarView` has one
`BtnMaximizeRestore` button whose `Style` (and therefore its normal/pressed
art) is swapped between `MaximizeButtonStyle` and `RestoreButtonStyle` in
code-behind, driven by subscribing to the window's `StateChanged` event -
this keeps the icon correct even when the state changes from something
other than this button (double-click, Windows Snap, `Win+Up`/`Win+Down`).

## Decision: Daisy is a non-hit-testable overlay, not baked into the button art

Matching the existing `face_screen.png` + `face.png` overlay pattern, the
logo button's frame (`logo_button.png`/`_pressed`) is the actual `Button`'s
skin, and `daisy.png` is a separate `Image` layered on top with
`IsHitTestVisible="False"` so clicks always reach the button underneath
rather than being swallowed by the overlay.
