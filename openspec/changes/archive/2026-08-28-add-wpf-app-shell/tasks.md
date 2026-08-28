## 1. Repo and project scaffold

- [x] 1.1 Add root `.gitignore` for .NET/VS (`bin/`, `obj/`, `.vs/`, user files).
- [x] 1.2 Create `src/CuteGreenCalculator/CuteGreenCalculator.csproj` targeting
      `net10.0-windows` with `UseWPF=true`, and `CuteGreenCalculator.sln` at repo root.
- [x] 1.3 Verify `dotnet build` succeeds from a clean clone.

## 2. Assets

- [x] 2.1 Copy all PNGs from the external assets folder into
      `src/CuteGreenCalculator/Assets/`.
- [x] 2.2 Include them as `Resource` items in the csproj; verify one loads via a
      pack URI in a throwaway test window before wiring real layout.

## 3. Window / view shell

- [x] 3.1 Create `MainWindow.xaml(.cs)` with only window-level properties (Title,
      SizeToContent, ResizeMode) hosting a `CalculatorView`.
- [x] 3.2 Create `CalculatorView.xaml(.cs)` (UserControl) that renders
      `background.png` upscaled with `BitmapScalingMode=NearestNeighbor`, with
      empty placeholder areas for the screen and button grid (no buttons yet).
- [x] 3.3 Run the app and confirm the background renders crisply at the chosen
      scale (no blur, no visible seams).

## 4. CI

- [x] 4.1 Add `.github/workflows/build.yml` running `dotnet build` on
      `push` and `pull_request` against `main`.
- [ ] 4.2 Confirm the workflow passes on the PR for this change.

## 5. Wrap-up

- [x] 5.1 Update this change's tasks to all-checked and confirm `dotnet build` /
      `dotnet run` both work from a fresh clone.
