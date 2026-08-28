## 1. Engine

- [x] 1.1 Create `CalculatorEngine.cs`: digit entry, decimal point, C, CE,
      +/- sign toggle, √, chained + − × ÷, and = (with repeat-last-operation
      behavior).
- [x] 1.2 Add divide-by-zero and sqrt-of-negative error handling (`IsError`,
      `Display` returns `"Error"`, only `Clear()` accepted while erroring).
- [x] 1.3 Add display formatting that avoids floating-point noise and
      excess trailing zeros.

## 2. Tests

- [x] 2.1 Add `tests/CuteGreenCalculator.Tests` xunit project, added to the
      `.sln`.
- [x] 2.2 Tests: basic arithmetic, chaining (`5+3+2=`), repeated `=`,
      leading zeros, decimal point handling (including a second `.` being
      ignored), +/- toggle, √ (including negative -> error), divide by
      zero -> error, C recovers from error, CE preserves a pending chain.
      (18 tests, all passing.)

## 3. UI wiring

- [x] 3.1 Instantiate `CalculatorEngine` in `CalculatorView.xaml.cs` and add
      `Click` handlers for every digit/operator/function/equals button
      (not the 45/90/180 speed-dials - those are `add-speed-dial-buttons`).
- [x] 3.2 Each handler calls the matching engine method and refreshes
      `DisplayText.Text`.
- [x] 3.3 Manually run the app and verify a full calculation end-to-end;
      confirmed a live button click updates the display via the engine.

## 4. Wrap-up

- [x] 4.1 `dotnet build` and `dotnet test` both clean (18/18 passing).
