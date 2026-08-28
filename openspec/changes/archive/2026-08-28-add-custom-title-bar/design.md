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

## Decision: Maximized letterbox is true transparency, not a solid fill

Initially the outer `Viewbox`'s letterbox bars (see the maximize decision
above) were filled with `Window.Background="#7B8A5E"`, the background art's
own border color, as a "looks intentional" compromise. Revisited during
review: the user asked whether the letterbox could show the desktop through
it instead, and confirmed they wanted true transparency once told the
trade-offs.

`AllowsTransparency="True"` (with `Background="Transparent"`) is required
for a per-pixel-alpha transparent window in WPF. The initial assumption was
that this would conflict with the existing `WindowChrome`-based edge/corner
resize (`CaptionHeight="0"`/`ResizeBorderThickness="6"`) and require
replacing it with a manual `WM_NCHITTEST` hook, similar in spirit to the
existing `WM_SIZING` aspect-ratio hook. That assumption was tested directly
rather than taken on faith: `AllowsTransparency="True"` was added to the
existing `WindowChrome` setup unchanged, then verified with the project's
established interactive-automation approach (not `SetWindowPos`/`MoveWindow`,
which don't exercise the real code paths) -

- A genuine `WM_SYSCOMMAND`/`SC_SIZE` edge-drag (single edge, away from
  screen edges to avoid the clamping quirk noted in project memory) still
  produced an exact aspect-ratio match.
- A raw mouse-down-move-up drag on `TitleBarView`'s `DragZone` (not
  `InvokePattern`, since it's a plain `MouseLeftButtonDown` handler) still
  moved the window by exactly the simulated delta.
- Maximize/restore via `InvokePattern` still swapped `WindowState` and the
  button icon correctly, and a real desktop screenshot (not `PrintWindow`,
  which renders transparent regions as opaque black and would have given a
  false negative here) confirmed the letterboxed area genuinely shows
  whatever is behind the window on the desktop.

So `WindowChrome` was kept as-is; only `MainWindow.xaml`'s `Background` and
`AllowsTransparency` changed. No manual hit-testing rewrite was needed.

**Trade-off accepted**: `AllowsTransparency="True"` disables the DWM drop
shadow around the window entirely (a known WPF limitation, unrelated to
`WindowChrome`). The user was informed of this before choosing true
transparency over the alternatives (keep the solid fill, or a semi-transparent
tint) and accepted it.

## Decision: Daisy is a non-hit-testable overlay, not baked into the button art

Matching the existing `face_screen.png` + `face.png` overlay pattern, the
logo button's frame (`logo_button.png`/`_pressed`) is the actual `Button`'s
skin, and `daisy.png` is a separate `Image` layered on top with
`IsHitTestVisible="False"` so clicks always reach the button underneath
rather than being swallowed by the overlay.
