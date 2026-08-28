## 1. Pin icon swap

- [x] 1.1 Confirm `star.png`/`star_pressed.png` are present in
      `Assets/` (already copied) and remove the "TEMPORARY placeholder"
      comment block above `AlwaysOnTopToggleStyle` in
      `Styles/PixelButtonStyles.xaml`.
- [x] 1.2 Point `AlwaysOnTopToggleStyle`'s `Tag`/`PixelButton.CheckedSource`
      at `star.png`/`star_pressed.png` instead of `heart.png`/
      `heart_screen.png`, and change `Width`/`Height` from `40`/`40` to
      `40`/`56` (8x scale of the 5x7 native art).

## 2. Uniform speed-dial skin + 270 button

- [x] 2.1 Confirm `mid_green_button.png`/`mid_green_button_pressed.png`
      are present in `Assets/` (already copied).
- [x] 2.2 In `Styles/PixelButtonStyles.xaml`, replace
      `MidGreenShortButtonStyle` and `MidGreenLongButtonStyle` with a
      single `MidGreenButtonStyle` (`Width="96" Height="80"`, `FontSize`
      matching the old speed-dial skins) backed by
      `mid_green_button.png`/`mid_green_button_pressed.png`.
- [x] 2.3 Delete the now-unreferenced
      `Assets/mid_green_button_short.png`,
      `Assets/mid_green_button_short_pressed.png`,
      `Assets/mid_green_button_long.png`,
      `Assets/mid_green_button_long_pressed.png`.
- [x] 2.4 In `Controls/CalculatorView.xaml`, switch `Btn45`/`Btn90`/
      `Btn180` to `MidGreenButtonStyle` and add a fourth `Btn270` button
      (`Content="270"`, same style, right-margin pattern matching the
      other 4-button rows).
- [x] 2.5 In `Controls/CalculatorView.xaml.cs`, wire
      `Btn270.Click += (_, _) => InsertAtCaret("270");` in `WireButtons()`
      alongside `Btn45`/`Btn90`/`Btn180`.

## 3. Operator column reorder

- [x] 3.1 In `Controls/CalculatorView.xaml`, move `BtnSqrt` out of row 3
      (leaving `C CE +/-`) and into row 7 as the 3rd button (between
      `BtnDecimal` and `BtnEquals`).
- [x] 3.2 Move `BtnDivide` from row 4 into row 3's vacated 4th slot;
      move `BtnMultiply` from row 5 into row 4; move `BtnSubtract` from
      row 6 into row 5; move `BtnAdd` from row 7 into row 6. Keep each
      button's `Style`/`Content`/`x:Name` unchanged, only its row and
      surrounding `Margin` change.
- [x] 3.3 Verify the final row order is: row3 `C CE +/- ÷`, row4
      `7 8 9 ×`, row5 `4 5 6 −`, row6 `1 2 3 +`, row7 `0 . √ =`.

## 4. Spec sync + verification

- [x] 4.1 `dotnet build` and `dotnet test` clean.
- [x] 4.2 Run the app (see `/run`) and visually confirm: the always-on-top
      toggle shows the star art in both states, all four speed-dial
      buttons are uniform width and aligned with the output screen, and
      the operator column reads ÷ × − + top-to-bottom with √ and =
      together on the bottom row.
- [x] 4.3 Stop and wait for the user's visual review before opening a PR.
