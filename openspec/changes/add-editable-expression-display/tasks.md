## Tasks

- [x] Rewrite `CalculatorEngine` around a full editable expression string
      (`TryInsert`, `SetText`, retargeted `ToggleSign`/`SquareRoot`/
      `ClearEntry`), keeping the legacy append-only API working.
- [x] Add new xunit tests covering mid-string edits, selection replacement,
      grammar rejection (double operators/decimals), and the
      speed-dial-after-equals replace behavior.
- [x] Verify all pre-existing tests still pass unchanged.
- [x] Replace the display `TextBlock` with a styled, editable `TextBox` in
      `CalculatorView.xaml`.
- [x] Wire digit/operator/decimal/speed-dial buttons through caret-aware
      insertion (`TryInsert`) instead of always-append.
- [x] Filter typed/pasted characters to calculator-understood characters via
      `PreviewTextInput`; route paste through the same filtering.
- [x] Sync engine state after native Backspace/Delete/Cut via `SetText`,
      reverting the TextBox if rejected.
- [x] Preserve Enter/Escape/F9/Ctrl+C/Ctrl+V shortcuts; drop the Delete=CE
      shortcut in favor of native forward-delete (CE stays on its button).
- [x] Add auto-shrink font sizing so long expressions never clip.
- [x] Add the copy icon button (new `CopyIconButtonStyle`) to the top-left
      of the output screen, wired to the same copy action as Ctrl+C/menu.
- [x] Manual verification: ran the app via UI Automation, exercised
      button-driven entry (5+3=8), CE preserving a pending operation,
      +/-, auto-shrink on a 19-character expression, the copy button
      (whole-value copy), keyboard caret repositioning + mid-string insert
      (13 -> 123), and Shift+Left selection + Ctrl+C copying exactly the
      selection. Also fixed the copy icon's margin/padding twice during
      review so it never overlaps the output screen's border or the
      display text.
- [x] `dotnet build` + `dotnet test` clean (39/39 passing).
