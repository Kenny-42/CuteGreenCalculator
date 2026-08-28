# Cute Green Calculator 🌼

A lightweight, pixel-art desktop calculator for Windows, built with WPF.
It looks like a cozy little handheld gadget and works like a real
four-function calculator — click it, type on your keyboard, or grab it
with the mouse.

![Cute Green Calculator screenshot](docs/screenshot.png)

## Installation & Setup

Grab the latest `CuteGreenCalculator.exe` from the
[Releases page](https://github.com/Kenny-42/CuteGreenCalculator/releases) — it's
a self-contained, single-file executable that runs on any Windows machine
without installing the .NET runtime. Download it and double-click to run.

## Features

- **Four-function math** — add, subtract, multiply, divide, with chained
  operations (`5 + 3 × 2 =`) and repeat-last-operation on repeated `=`.
- **Square root** (`√`) and sign toggle (`+/-`).
- **Speed-dial buttons** — `45`, `90`, `180`, `270` type those digits straight
  into the display in one click, handy for quick angle entry.
- **Full keyboard support** — digits, `.`, `+ - * /`, Enter for `=`, Escape
  for `C`, Delete for `CE`, Backspace to delete a character, F9 for `+/-`,
  and `@` for `√`.
- **Copy/paste** — copy the display with Ctrl+C, paste a number back in
  with Ctrl+V, click the copy button next to the display, or use the
  right-click context menu on the display.
- **Always-on-top toggle** — pin the calculator above other windows.
- **Heart toggles** — 4 clickable hearts in the status row form a
  left-to-right threshold group: click one to toggle it and everything to
  its right on, click the leftmost lit heart again to clear the group.
  Purely decorative.
- Crisp, unblurred pixel art at any Windows display scale (Per-Monitor-V2
  DPI awareness).

## Tech stack

- **.NET 10** / **WPF** (`net10.0-windows`), no third-party UI libraries.
- **xunit** for the test suite.
- **[OpenSpec](openspec/)** for spec-driven planning — each feature is
  proposed, specified, and archived as a change under `openspec/`, one per
  GitHub issue.
- **GitHub Actions** for CI (build + test on every push) and releases
  (self-contained `win-x64` publish attached to a GitHub Release on tag push).

### Building from source

**Prerequisites**: Windows, and the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```sh
# Build
dotnet build

# Run
dotnet run --project src/CuteGreenCalculator

# Run the test suite
dotnet test
```

The app project lives at [src/CuteGreenCalculator/](src/CuteGreenCalculator/);
the calculator's core arithmetic logic (independent of any UI) is in
[CalculatorEngine.cs](src/CuteGreenCalculator/CalculatorEngine.cs), covered by
xunit tests in [tests/CuteGreenCalculator.Tests/](tests/CuteGreenCalculator.Tests/).

## License

[MIT](LICENSE) — see the LICENSE file for details.
