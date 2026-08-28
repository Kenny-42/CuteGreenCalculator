## 1. Heart toggle skin

- [x] 1.1 Copy `heart_pressed.png` into `src/CuteGreenCalculator/Assets/`
  (picked up automatically by the existing `Assets\**\*.png` wildcard in the
  csproj).
- [x] 1.2 In `PixelButtonStyles.xaml`, add `HeartToggleStyle` (56x48,
  reusing `PixelToggleButtonTemplate`/`CheckedSource`, matching the always-
  on-top pin toggle's pattern): `Tag`=`heart.png`, `CheckedSource`=
  `heart_pressed.png`.

## 2. Clickable heart group

- [x] 2.1 In `CalculatorView.xaml`, replace the 4 static heart `Image`
  elements with 4 named `ToggleButton`s (`Heart0`..`Heart3`) using
  `HeartToggleStyle`.
- [x] 2.2 In `CalculatorView.xaml.cs`, add `WireHearts()` (wires each
  heart's `Click` to `OnHeartClicked(index)`), a `_heartThreshold` field
  (nullable int, the leftmost toggled heart's index or null if none), and
  `RefreshHearts()` which sets every heart's `IsChecked` from
  `_heartThreshold`. `OnHeartClicked(index)` sets `_heartThreshold = index`,
  or clears it to `null` if `index` already equals the current threshold
  (clicking the leftmost toggled heart again untoggles the whole group).

## 3. Verification

- [x] 3.1 `dotnet build` succeeds with no warnings/errors.
- [x] 3.2 `dotnet test` passes with no regressions (39/39).
- [x] 3.3 Verify via UI Automation + DPI-aware screenshots: all 4 hearts
  start normal; clicking heart 4 toggles only heart 4; clicking heart 3
  toggles hearts 3-4 (heart 4 stays toggled, 1-2 stay normal); clicking
  heart 2 toggles hearts 2-4; clicking heart 2 again (the leftmost toggled
  heart) clears all 4 back to normal.
