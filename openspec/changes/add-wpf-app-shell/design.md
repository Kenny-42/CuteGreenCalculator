## Context

Greenfield WPF project. The user wants a standard title bar for now but plans to
replace it with a fully custom pixel-art borderless frame later, and doesn't want
that future change to require reworking the calculator UI.

## Goals / Non-Goals

**Goals:**
- Keep `MainWindow` "dumb": window-level concerns only (title, icon, topmost, sizing).
- Put all calculator visuals/behavior in `CalculatorView`, a `UserControl` that does
  not know or care what kind of window hosts it.
- Establish an `Assets/` convention and a nearest-neighbor scaling convention that
  every later change reuses.

**Non-Goals:**
- Not implementing the custom borderless frame now.
- Not implementing button styles, fonts, or calculator logic in this change.

## Decisions

- **Target framework**: `net10.0-windows`, since .NET 10 SDK is the only SDK
  installed and WPF is fully supported on it. `<UseWPF>true</UseWPF>`.
- **Project layout**: `src/CuteGreenCalculator/CuteGreenCalculator.csproj` (app),
  a `.sln` at repo root referencing it. A test project is added in the calculator
  engine change, not this one.
- **Asset inclusion**: PNGs copied into `Assets/` inside the project and included
  with `<Resource Include="Assets\**\*.png" />` so they're pack-URI addressable
  (`pack://application:,,,/Assets/xxx.png`) without a separate copy-to-output step.
- **Pixel-perfect scaling**: set `RenderOptions.BitmapScalingMode="NearestNeighbor"`
  on the root `Image`/`Border` hosting the background art (and later, on every
  button image) rather than globally, so the setting is visible next to each usage.
- **Window/View split**: `MainWindow.xaml` contains only a `<local:CalculatorView/>`
  plus window chrome properties (Title, Icon, SizeToContent, ResizeMode). All pixel
  art layout lives in `CalculatorView.xaml`.

## Risks / Trade-offs

- Fixed, non-resizable window sizing is simplest for pixel art (avoids scaling
  artifacts from arbitrary resize) but sacrifices resizability — acceptable for v1
  since the source art is a fixed-size mockup.
