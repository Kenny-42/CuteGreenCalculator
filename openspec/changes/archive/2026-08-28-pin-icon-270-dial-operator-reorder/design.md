## Context

Three unrelated but small UI changes bundled under one issue (#19) because
they all become possible now that new art exists: `star.png`/
`star_pressed.png` (pin icon) and `mid_green_button.png`/
`mid_green_button_pressed.png` (uniform speed-dial skin), both already
copied into `Assets/`. The operator reorder needs no new art at all.

## Goals / Non-Goals

**Goals:**
- Replace the heart placeholder on `AlwaysOnTopToggleStyle` with real pin
  art, scaled the same way every other sprite in the app is scaled (8x
  integer, nearest-neighbor).
- Let all four speed-dial buttons (45/90/180/270) share one uniform skin
  and width, still aligning edge-to-edge with the output screen.
- Reorder the operator column purely by moving existing `<Button>` elements
  between `StackPanel` rows - no new bindings, no new code-behind logic
  beyond wiring `Btn270`.

**Non-Goals:**
- No change to `CalculatorEngine` or any calculation behavior.
- No change to the status-row heart display (`heart.png`/`heart_screen.png`
  used at `CalculatorView.xaml` lines 74-84) - that's a separate "life"
  indicator, not the always-on-top toggle, and is out of scope.
- No new capability for "270" beyond the existing speed-dial digit-insert
  pattern already established for 45/90/180.

## Decisions

- **Pin icon sizing**: `star.png` is 5x7 native. Every other icon in the
  app (e.g. `copy_button.png`, 4x4 native -> 32x32) uses an 8x integer
  scale, so the toggle becomes `Width="40" Height="56"` instead of the old
  hardcoded 40x40 box (which stretched the heart's 7x6 art non-uniformly).
  The toggle's position (`HorizontalAlignment="Right"`,
  `VerticalAlignment="Top"`, fixed `Margin`) is absolute over the
  background art and doesn't affect any other element's layout, so the
  height change is safe.
- **Speed-dial uniform skin**: `mid_green_button.png` is 12x10 native, the
  same native size as every number/operator button
  (`light_green_button.png`, `light_pink_button.png`, etc.), so it uses the
  same `Width="96" Height="80"` as `LightGreenButtonStyle` and friends. The
  old `MidGreenShortButtonStyle` (112x80) and `MidGreenLongButtonStyle`
  (176x80) are deleted rather than left dead, since nothing else
  references them and keeping unused XAML/asset pairs around just invites
  drift. Row math: 4 buttons x 96 + 3 gaps x 16 = 432, matching the output
  screen width and every other 4-column row.
- **Operator reorder is position-only**: each moved `<Button>` keeps its
  existing `x:Name`, `Style`, `Content`, and Click wiring untouched; only
  its `StackPanel` row (and `Margin`, where its position in the row
  changes) changes. This keeps the diff obviously behavior-preserving for
  everything except which row each button renders in.
- **Btn270 wiring**: identical pattern to `Btn45`/`Btn90`/`Btn180` -
  `Btn270.Click += (_, _) => InsertAtCaret("270");` alongside the others in
  `WireButtons()`.

## Risks / Trade-offs

- [Deleting `MidGreenShortButtonStyle`/`MidGreenLongButtonStyle` and their
  PNGs is a breaking change to anyone with in-flight branches referencing
  them] -> Low risk (solo project, `git grep` confirmed no other
  references); acceptable since dead styles/assets are worse long-term
  clutter than a one-time removal.
- [Reordering rows changes five `x:Name`d buttons' visual position at
  once, easy to mis-wire a Style/Content pairing] -> Mitigated by moving
  whole `<Button>` elements verbatim between `StackPanel`s rather than
  retyping their attributes, then a manual visual check before PR.

## Open Questions

- (none - user will visually review the running app before the PR is
  opened, per their request.)
