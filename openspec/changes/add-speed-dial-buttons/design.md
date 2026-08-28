## Context

The speed-dial buttons are a UI convenience, not a new engine concept: "press
45" should be indistinguishable from the engine's perspective from "press 4,
then press 5". `CalculatorEngine.InputDigit(char)` already handles the
start-new-entry / append / leading-zero logic correctly for that case.

## Goals / Non-Goals

**Goals:**
- Pressing 45, 90, or 180 has exactly the same effect on `_currentEntry` as
  pressing its digits individually, including mid-entry append behavior
  (e.g. current entry `7`, press `45` -> `745`) and fresh-entry replace
  behavior (e.g. right after `=`, press `90` -> `90`, not `790`).
- No new `CalculatorEngine` methods or state.

**Non-Goals:**
- No angle/trig semantics - these are plain digit shortcuts, not calculator
  functions.

## Decisions

- **Implementation**: each speed-dial handler iterates the button's literal
  digit string and calls `_engine.InputDigit(c)` per character inside the
  existing `Handle(Action)` helper, e.g.
  `BtnEqualsStyleExample.Click += (_, _) => Handle(() => InputDigits("45"));`
  with a small private `InputDigits(string)` helper shared by all three
  buttons to avoid repeating the loop three times.
- **Placement**: wired in `WireButtons()` alongside the other buttons, kept
  visually grouped/comment-labeled as the speed-dial group since they're
  conceptually different (shortcuts, not raw digits or operators).

## Risks / Trade-offs

- None of note - this is a thin, low-risk UI wiring change reusing
  well-tested engine behavior.
