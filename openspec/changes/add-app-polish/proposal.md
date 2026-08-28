## Why

The calculator is functionally complete (#1-#5) but still looks and reads
like a work-in-progress: no custom exe icon, default assembly metadata, no
verified visual parity with the reference mockup, and a one-line README.
GitHub issue #6 asks for a final presentation/documentation pass so the app
looks and reads like a finished product.

## What Changes

- A window/exe icon derived from existing pixel art (the `face.png` sprite)
  is added as a multi-resolution `.ico` and wired into the csproj
  (`<ApplicationIcon>`) and `MainWindow` (`Icon=`).
- Assembly metadata (product name, description, version, company/author) is
  set in `CuteGreenCalculator.csproj`.
- A side-by-side visual comparison against the mockup
  (`pixilart-drawing.png`) is performed on a real running instance; any
  noticeable spacing/sizing/color drift from the mockup is fixed in
  `CalculatorView.xaml` / `PixelButtonStyles.xaml`.
- `README.md` is rewritten with a real description, feature list, screenshot,
  and build/run instructions.

## Capabilities

### New Capabilities
- None (this change is presentation/documentation polish, not new
  application behavior).

### Modified Capabilities
- None. No `CalculatorEngine` or input-handling behavior changes.

## Impact

- `src/CuteGreenCalculator/CuteGreenCalculator.csproj`: `<ApplicationIcon>`,
  product/description/version/company metadata.
- `src/CuteGreenCalculator/Assets/app.ico` (new): multi-res icon generated
  from `face.png`.
- `src/CuteGreenCalculator/MainWindow.xaml`: `Icon` attribute.
- `src/CuteGreenCalculator/Controls/CalculatorView.xaml` /
  `Styles/PixelButtonStyles.xaml`: only touched if the visual diff pass finds
  mismatches against the mockup.
- `README.md`: full rewrite.
