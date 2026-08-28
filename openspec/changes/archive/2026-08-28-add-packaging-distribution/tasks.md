## 1. Publish configuration

- [x] 1.1 Add self-contained single-file win-x64 publish properties to
  `CuteGreenCalculator.csproj` (`SelfContained`, `PublishSingleFile`,
  `IncludeNativeLibrariesForSelfExtract`), guarded by
  `Condition="'$(RuntimeIdentifier)' != ''"` so plain `dotnet
  build`/`run`/`test` are unaffected
- [x] 1.2 Run `dotnet publish -r win-x64 -c Release` and confirm a single
  standalone `.exe` is produced in the publish output directory - confirmed:
  one ~140MB `CuteGreenCalculator.exe` (+`.pdb`) in
  `bin/Release/net10.0-windows/win-x64/publish/`

## 2. Standalone verification

- [x] 2.1 Launch the published `.exe` directly and confirm the app starts and
  functions normally (not just that it builds) - launched standalone,
  UI-Automation-clicked button "7", DPI-aware screenshot confirms the display
  correctly shows "7"
- [x] 2.2 Confirm it doesn't depend on the dev machine's installed SDK - exe
  size (~140MB, vs a few hundred KB for the framework-dependent build)
  confirms the .NET runtime is bundled in

## 3. Release automation

- [x] 3.1 Add `.github/workflows/release.yml`: triggers on `v*` tag push and
  `workflow_dispatch` (with a required `tag_name` input for manual runs,
  since `softprops/action-gh-release` needs a tag to attach to), builds the
  self-contained win-x64 exe, renames it to
  `CuteGreenCalculator-win-x64.exe`, attaches it to a GitHub Release
- [x] 3.2 Sanity-check the workflow YAML - validated as well-formed YAML
  (`yaml.safe_load`); actionlint wasn't available locally, so also did a
  careful manual review against `build.yml`'s established pattern (same
  `windows-latest` runner, same `setup-dotnet` action/version) since a
  tag-triggered workflow is hard to dry-run locally

## 4. Documentation

- [x] 4.1 Add a "Publishing a release build" section to `README.md` with the
  exact `dotnet publish` command
- [x] 4.2 Mention the automated release workflow (tag push triggers a GitHub
  Release with the attached exe)

## 5. Wrap-up

- [x] 5.1 `dotnet build` and `dotnet test` both still pass (25/25 tests)
- [x] 5.2 Open PR against `main` with "Closes #7", wait for CI, self-merge
  per project workflow - PR #14, CI passed, squash-merged, branch deleted
