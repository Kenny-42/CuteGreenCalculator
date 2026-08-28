## Why

The repo currently has no code — just a README. Before any calculator UI or logic
can be built, the project needs a running WPF app shell with the pixel art assets
wired in and rendering crisply, plus CI so every later PR is validated by a real
build before it's merged.

## What Changes

- Create the WPF solution/project (net10.0-windows) with standard .NET/VS gitignore.
- Copy all PNG assets into the repo under `Assets/` and include them as build resources.
- Add a `CalculatorView` UserControl (separate from `MainWindow`) that renders the
  calculator body background art upscaled with nearest-neighbor scaling.
- Add a GitHub Actions workflow that runs `dotnet build` on push and pull_request.

## Capabilities

### New Capabilities
- `app-shell`: The base WPF application window, project structure, asset pipeline,
  and the window/calculator-face separation that later changes build on.

### Modified Capabilities
- (none — this is the first change)

## Impact

- New files: `CuteGreenCalculator.sln`, `src/CuteGreenCalculator/*`, `Assets/*.png`,
  `.gitignore`, `.github/workflows/build.yml`.
- No existing code affected (greenfield).
