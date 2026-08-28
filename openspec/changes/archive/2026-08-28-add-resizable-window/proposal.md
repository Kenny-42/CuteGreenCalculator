## Why

The window is currently fixed-size (`SizeToContent="WidthAndHeight"`, `ResizeMode="CanMinimize"`), and the button grid rows are narrower than the output screen above them, so their left/right edges don't line up. Issue #15 asks for a resizable window that scales the whole face uniformly, plus fixed alignment so the layout reads as intentional. This should land before the custom borderless title bar work, which assumes a resizable window.

## What Changes

- `MainWindow` becomes resizable: `ResizeMode="CanResize"`, `SizeToContent` removed, `MinWidth`/`MinHeight` set to a sensible floor, and the window's resize is constrained to the calculator face's native aspect ratio (496:840) so dragging any edge/corner never distorts the art.
- `CalculatorView`'s fixed 496x840 `Grid` host is wrapped in a `Viewbox` (`Stretch="Uniform"`) so the whole face scales as one image with the window instead of staying pinned at 496x840.
- Button row spacing in `CalculatorView.xaml` is corrected so the leftmost and rightmost button edges in every row line up with the output screen's left/right edges, with even gaps between columns in each row.
- No new art assets.

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
- `app-shell`: window sizing behavior changes from fixed-size/`CanMinimize` to resizable-with-locked-aspect-ratio, and the calculator face now scales via a `Viewbox` instead of staying pixel-fixed.
- `calculator-layout`: button grid column alignment requirement changes from "centered independently per row" to "edges aligned to the output screen's edges, even gaps."

## Impact

- `src/CuteGreenCalculator/MainWindow.xaml` / `MainWindow.xaml.cs` (resize mode, min size, aspect-ratio lock via `WM_SIZING`).
- `src/CuteGreenCalculator/Controls/CalculatorView.xaml` (Viewbox wrapper, button row margins).
- No changes to `CalculatorEngine`, button click wiring, or art assets.
