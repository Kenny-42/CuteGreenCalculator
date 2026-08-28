## Context

Issue #6 is a polish/documentation pass, not new behavior. No engine or
input-handling capability changes, so this change has no specs delta.

## Icon generation

There's no existing `.ico` asset or icon-generation tooling in the repo.
`face.png` (72x48, transparent background) is the natural source since it's
already the calculator's "face" - the most recognizable single sprite.
It'll be composed onto a square canvas and resized down to the standard
Windows icon sizes (16/32/48/256) using a one-off ImageMagick/`System.Drawing`
script (not committed - same pattern as the DPI-aware screenshot helper used
in prior issues), producing a single multi-resolution `.ico` file that's
committed as a build asset.

## Visual QA approach

Compare a fresh DPI-aware screenshot of the running app directly against
`pixilart assets/pixilart-drawing.png` (the original mockup). Any fix here is
expected to be small (a margin/color tweak in existing XAML/styles) since
issues #1-#2 already built the face layout directly against this mockup.

## No spec delta

This change doesn't add or modify any `CalculatorEngine` or UI-behavior
capability, so no `specs/` delta is included.
