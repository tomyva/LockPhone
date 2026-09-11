# LockPhone

A minimal C# / .NET for Android app for Pixel 6a-class devices. It has an enable switch, an Android Device Admin permission flow for `LockNow()`, an accessibility-service setup shortcut, and a local ad placeholder reading **HI This is an Ad**.

## Build

```powershell
dotnet build -f net10.0-android
```

The checked-in target SDK is API 36 because that is the installed Android SDK in this workspace. It is forward-compatible for testing on a newer Android release, but the target can be raised when the Android 17/API SDK is available.

## Important platform constraint

Android does not permit a normal app to monitor taps across every other app. The double-tap detector is therefore an `AccessibilityService`, which the user has to explicitly enable in Android Settings. It also needs Device Admin activation before the app may lock the screen. On Android versions/builds where accessibility double-tap gestures are reserved for touch exploration, a production implementation should use a user-visible accessibility interaction or an OEM-supported gesture API instead.

The ad is intentionally only a visual placeholder—no SDK, tracking, or network traffic is included.
