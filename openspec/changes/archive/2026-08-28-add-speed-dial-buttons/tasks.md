## 1. UI wiring

- [x] 1.1 Add a private `InputDigits(string)` helper to `CalculatorView.xaml.cs`
      that calls `_engine.InputDigit(c)` for each character.
- [x] 1.2 Wire `Btn45.Click`, `Btn90.Click`, `Btn180.Click` in `WireButtons()`
      to call `Handle(() => InputDigits("45"))` etc.

## 2. Verification

- [x] 2.1 `dotnet build` and `dotnet test` clean (18/18 passing).
- [x] 2.2 Manually run the app and confirm 45/90/180 insert correctly both on
      a fresh entry and mid-entry.
