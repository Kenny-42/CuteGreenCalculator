## 1. Icon and metadata

- [x] 1.1 Generate a multi-resolution `.ico` (16/32/48/256) from `face.png`
  (or a composed face+screen crop) and add it as
  `src/CuteGreenCalculator/Assets/app.ico`
- [x] 1.2 Set `<ApplicationIcon>` in `CuteGreenCalculator.csproj`
- [x] 1.3 Set `Icon="Assets/app.ico"` on `MainWindow.xaml` so the window's
  title bar/taskbar entry also shows it (not just the exe file icon)
- [x] 1.4 Set assembly metadata in the csproj: `<Product>`,
  `<AssemblyTitle>`, `<Description>`, `<Version>`/`<AssemblyVersion>`,
  `<Company>`/`<Authors>`
- [x] 1.5 `dotnet build` succeeds and the taskbar/title bar icon is verified
  visually

## 2. Visual QA pass against the mockup

- [x] 2.1 Launch the app and screenshot it (DPI-aware capture per project
  known quirk)
- [x] 2.2 Compare side-by-side against
  `pixilart assets/pixilart-drawing.png`: overall proportions, button
  colors/positions, screen text color/alignment, always-on-top toggle
  placement
- [x] 2.3 Fix any noticeable mismatches found (XAML/style tweaks only - no
  behavior changes) - **none found**: layout, colors, and proportions
  already match the mockup closely (issues #1-#2 built directly against it).
  The always-on-top toggle is intentionally new vs. the mockup (added in
  #5). No XAML/style changes made.
- [x] 2.4 Re-screenshot to confirm fixes - n/a, no fixes needed

## 3. README rewrite

- [x] 3.1 Real project description and feature list (four-function math,
  square root, +/-, speed-dial angle buttons, keyboard input, copy/paste,
  always-on-top)
- [x] 3.2 Screenshot of the running app embedded (`docs/screenshot.png`)
- [x] 3.3 Build/run instructions (`dotnet build`, `dotnet run --project
  src/CuteGreenCalculator`, prerequisites: .NET 10 SDK, Windows)
- [x] 3.4 Mention the test project (`dotnet test`)

## 4. Wrap-up

- [x] 4.1 `dotnet build` and `dotnet test` both pass
- [x] 4.2 Open PR against `main` with "Closes #6", wait for CI, self-merge
  per project workflow
