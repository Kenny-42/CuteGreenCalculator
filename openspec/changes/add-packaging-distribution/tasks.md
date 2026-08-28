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

- [ ] 3.1 Add `.github/workflows/release.yml`: triggers on `v*` tag push and
  `workflow_dispatch`, builds the self-contained win-x64 exe, attaches it to
  a GitHub Release
- [ ] 3.2 Sanity-check the workflow YAML (actionlint/`gh workflow` if
  available, or careful manual review) since a tag-triggered workflow is hard
  to dry-run locally

## 4. Documentation

- [ ] 4.1 Add a "Publishing a release build" section to `README.md` with the
  exact `dotnet publish` command
- [ ] 4.2 Mention the automated release workflow (tag push triggers a GitHub
  Release with the attached exe)

## 5. Wrap-up

- [ ] 5.1 `dotnet build` and `dotnet test` both still pass
- [ ] 5.2 Open PR against `main` with "Closes #7", wait for CI, self-merge
  per project workflow
