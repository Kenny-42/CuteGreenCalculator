## 1. Font

- [x] 1.1 Add `PressStart2P-Regular.ttf` and its `OFL.txt` license under
      `Assets/Fonts/`, included as a `Resource` build item.
- [x] 1.2 Reference the font via a shared `FontFamily` resource and confirm it
      renders (not falling back to a system font) in a quick manual check.

## 2. Button styles

- [x] 2.1 Create `Styles/PixelButtonStyles.xaml` with a shared `ControlTemplate`
      and six named styles (light_green, mid_green_long, mid_green_short,
      light_pink, mid_pink, dark_pink), each with correct Width/Height and
      pressed-state art swap, no hover visual.
- [x] 2.2 Merge the dictionary into `App.xaml` resources.

## 3. Layout

- [x] 3.1 Add the status row: `face_screen` with `face.png` centered inside,
      and `heart_screen` with 4 `heart.png` sprites centered inside.
- [x] 3.2 Add the output screen (`output_screen.png`) with an overlaid,
      right-aligned `DisplayText` TextBlock in the pixel font (placeholder "0").
- [x] 3.3 Add the speed-dial row: 45 (short), 90 (short), 180 (long).
- [x] 3.4 Add the 5-row, 4-column button grid: `C CE +/- √` / `7 8 9 ÷` /
      `4 5 6 ×` / `1 2 3 −` / `0 . + =`, using the matching styles per button.
- [x] 3.5 Run the app, screenshot it, and compare against the reference mockup;
      adjust spacing/margins/font size until it reads as the same layout.

## 4. Wrap-up

- [x] 4.1 Confirm `dotnet build` is clean and all buttons show correct
      pressed-state art on click (manual check).
- [x] 4.2 Add Per-Monitor-V2 `app.manifest` so the pixel art renders crisply
      (not OS bitmap-stretched) on scaled displays.
