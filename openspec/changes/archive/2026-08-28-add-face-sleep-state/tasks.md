## 1. Window focus tracking

- [x] 1.1 In `MainWindow.xaml.cs`, subscribe to `Activated`/`Deactivated` and
      call `Calculator.SetFocused(bool)` directly (MainWindow -> CalculatorView
      is a direct method call here rather than an event, since the direction
      of flow only ever goes one way, unlike the always-on-top
      toggle-originates-in-the-view case).

## 2. Face state swap in CalculatorView

- [x] 2.1 Name the face `Image` element in `CalculatorView.xaml` (`FaceImage`).
- [x] 2.2 Add a `FaceState` enum (`Awake`, `Asleep`) and a `SetFaceState`
      method in `CalculatorView.xaml.cs` that maps state to asset path via a
      lookup (`FaceStateAssets`), so future states are added by extending the
      enum + lookup only.
- [x] 2.3 Expose `public void SetFocused(bool focused)` on `CalculatorView`
      calling `SetFaceState(focused ? FaceState.Awake : FaceState.Asleep)`.
- [x] 2.4 Wire `MainWindow`'s `Activated`/`Deactivated` handlers to call this.

## 3. Art asset

- [x] 3.1 Add `Assets/face_sleep.png` (copied from the shared pixilart
      assets folder). No csproj change needed - the existing
      `<Resource Include="Assets\**\*.png" />` wildcard already picks it up.

## 4. Verification

- [x] 4.1 `dotnet build` succeeds with no warnings/errors.
- [x] 4.2 `dotnet test` passes (no engine/logic changes expected).
- [x] 4.3 Launch the app and verify visually: face shows normal `face.png`
      when the window is focused, switches to `face_sleep.png` when another
      window is focused, and switches back immediately on refocus.
