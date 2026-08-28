## Context

Issue #7 is packaging/distribution tooling, not new application behavior. No
engine or input-handling capability changes, so this change has no specs
delta.

## Publish configuration

WPF app targeting `net10.0-windows`, so `win-x64` is the natural single RID
to target (WPF is Windows-only; no need for a multi-RID matrix).
`PublishSingleFile=true` + `SelfContained=true` bundles the .NET runtime into
one `.exe` so the target machine needs nothing installed.
`IncludeNativeLibrariesForSelfExtract=true` is needed alongside
`PublishSingleFile` so native WPF dependencies extract correctly at runtime
rather than being left beside the exe.

These properties are added as plain `<PropertyGroup>` properties in the
csproj guarded by nothing extra — they only take effect when `-p:PublishSingleFile=true` etc. are passed on the `dotnet publish` command line (a
dedicated `.pubxml` publish profile is the standard MSBuild way to do this,
but a documented CLI command is simpler for this project's size and keeps the
csproj readable, matching how `README.md` already documents `dotnet
build`/`dotnet run`/`dotnet test` directly).

## Release workflow

New `.github/workflows/release.yml`, separate from `build.yml` (which stays
untouched — it's the fast PR/push CI gate). The release workflow:
- Triggers on pushing a tag matching `v*`, or `workflow_dispatch` for manual
  runs.
- Runs on `windows-latest` (matches `build.yml`).
- `dotnet publish` with the win-x64 self-contained single-file flags,
  `-c Release`.
- Uploads the resulting `.exe` to a new GitHub Release for that tag via
  `softprops/action-gh-release` (a widely-used, well-maintained action for
  attaching build artifacts to releases — avoids hand-rolling the GitHub API
  calls).

## Verification

After publishing locally, the produced `.exe` is copied out of the repo
folder (or run with the dev SDK's build output cleared/not on PATH) and
launched to confirm it starts without requiring the SDK — the standard way to
prove a self-contained publish is actually self-contained.
