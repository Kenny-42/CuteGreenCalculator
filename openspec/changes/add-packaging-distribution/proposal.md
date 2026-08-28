## Why

The calculator is feature-complete and polished (#1-#6) but can currently
only be run via `dotnet run`/`dotnet build`, which requires the .NET 10 SDK
installed. GitHub issue #7 asks for a shareable, self-contained Windows
executable and an automated release pipeline so the app can be handed to
someone without a dev environment.

## What Changes

- A publish configuration for a self-contained, single-file, win-x64 build
  (`SelfContained=true`, `PublishSingleFile=true`, `RuntimeIdentifier=win-x64`)
  is added to the app csproj so `dotnet publish` produces one standalone
  `.exe`.
- `README.md` documents the exact `dotnet publish` command for producing a
  release build locally.
- A new GitHub Actions workflow (`.github/workflows/release.yml`) builds this
  self-contained artifact and attaches it to a GitHub Release, triggered by
  pushing a version tag (`v*`) or manual dispatch.
- The produced `.exe` is verified to run standalone (no reliance on the dev
  machine's installed SDK/runtime).

## Capabilities

### New Capabilities
- None (this change is build/packaging tooling, not new application
  behavior).

### Modified Capabilities
- None. No `CalculatorEngine` or UI-behavior changes.

## Impact

- `src/CuteGreenCalculator/CuteGreenCalculator.csproj`: publish-profile
  properties (`RuntimeIdentifier`, `SelfContained`, `PublishSingleFile`,
  `PublishReadyToRun` optional, `IncludeNativeLibrariesForSelfExtract`).
- `.github/workflows/release.yml` (new): tag/manual-triggered publish +
  GitHub Release upload.
- `README.md`: "Publishing a release build" section with the `dotnet
  publish` command.
