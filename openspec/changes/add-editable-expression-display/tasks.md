## Tasks

- [x] Rewrite `CalculatorEngine` around a full editable expression string
      (`TryInsert`, `SetText`, retargeted `ToggleSign`/`SquareRoot`/
      `ClearEntry`), keeping the legacy append-only API working.
- [x] Add new xunit tests covering mid-string edits, selection replacement,
      grammar rejection (double operators/decimals), and the
      speed-dial-after-equals replace behavior.
- [x] Verify all pre-existing tests still pass unchanged.
- [ ] Replace the display `TextBlock` with a styled, editable `TextBox` in
      `CalculatorView.xaml`.
- [ ] Wire digit/operator/decimal/speed-dial buttons through caret-aware
      insertion (`TryInsert`) instead of always-append.
- [ ] Filter typed/pasted characters to calculator-understood characters via
      `PreviewTextInput`; route paste through the same filtering.
- [ ] Sync engine state after native Backspace/Delete/Cut via `SetText`,
      reverting the TextBox if rejected.
- [ ] Preserve Enter/Escape/F9/Ctrl+C/Ctrl+V shortcuts; drop the Delete=CE
      shortcut in favor of native forward-delete (CE stays on its button).
- [ ] Add auto-shrink font sizing so long expressions never clip.
- [ ] Add the copy icon button (new `CopyIconButtonStyle`) to the top-left
      of the output screen, wired to the same copy action as Ctrl+C/menu.
- [ ] Manual verification: run the app, exercise click-to-position-cursor,
      mid-string insert/delete, selection + copy, paste (valid/invalid),
      auto-shrink on a long expression, and the copy button.
- [ ] `dotnet build` + `dotnet test` clean.
