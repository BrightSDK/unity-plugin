# unity-plugin – Full Documentation

## Overview

unity-plugin is the BrightSDK Unity integration package that automates SDK download/update during Unity builds and exposes runtime consent APIs through MonoBehaviour helpers.

Repository: https://github.com/BrightSDK/unity-plugin
Package name: com.brightdata.brightsdkunityupdater
Version: 1.0.16

Supported target platforms:

- Android
- iOS / tvOS (Apple Mobile)
- macOS (Apple Desktop)
- Windows

---

## What the Plugin Does

1. Hooks into Unity prebuild pipeline.
2. Detects the build target platform.
3. Downloads the relevant Bright SDK archive from CDN.
4. Extracts SDK files into Assets/Plugins platform folders.
5. Removes obsolete SDK files to avoid stale-version conflicts.
6. Configures platform compatibility/import settings for native frameworks.

Core prebuild class:

- Editor/BrightSDKLoaderPrebuild.cs (implements IPreprocessBuildWithReport)

---

## Build-Time Automation Pipeline

### Entry point: BrightSDKLoaderPrebuild

On every build (`OnPreprocessBuild`):

- Checks if platform is supported.
- Loads latest versions from https://bright-sdk.com/sdk_api/sdk/versions (with one-day cache).
- Picks downloader + extractor implementation per BuildTarget.
- Downloads archive to Library/BrightSdkCache.
- Extracts and installs into Assets/Plugins/... folders.

Platform mapping implemented in constructor:

- Android -> AndroidSDKArchiveDownloader + AndroidBrightSDKExtractor
- iOS/tvOS -> AppleMobileSDKArchiveDownloader + AppleMobileBrightSDKExtractor
- StandaloneOSX -> AppleDesktopSDKArchiveDownloader + AppleDesktopBrightSDKExtractor
- StandaloneWindows/StandaloneWindows64 -> WindowsSDKArchiveDownloader + WindowsBrightSDKExtractor

---

## Version Resolution

Source:

- Editor/Utils/BrightSDKVersions.cs
- Editor/Utils/BrightSDKConfig.cs
- Editor/BrightSDK.json

Behavior:

- Remote versions fetched from Bright API and cached in:
  - Library/BrightSdkCache/sdk_versions.json
- Cache invalidated after 24 hours.
- Per-platform pinned versions can be set in BrightSDK.json:
  - versions.android
  - versions.appleMobile
  - versions.appleDesktop
  - versions.windows
- null means use latest remote version.

Default config in repository:

```json
{
  "versions": {
    "android": null,
    "appleMobile": null,
    "appleDesktop": null,
    "windows": null
  }
}
```

Archive names by platform:

- Android: bright_sdk_android-{version}.tar.gz
- iOS/tvOS: bright_sdk_ios-{version}.zip
- macOS: bright_sdk_macos_unity-{version}.zip
- Windows: bright_sdk_win-{version}.zip

All downloaded from:

- https://cdn.bright-sdk.com/static/

---

## Extractors and Install Paths

### Android

Extractor: AndroidBrightSDKExtractor

- Removes old `bright_sdk*.aar` in target folder.
- Unpacks `.tar.gz` (GZip + Tar via Unity SharpZipLib).
- Finds first `.aar` recursively.
- Copies as:
  - Assets/Plugins/Android/bright_sdk.aar

### Apple Mobile (iOS/tvOS)

Extractor: AppleMobileBrightSDKExtractor

- Uses macOS `ditto` to unzip and copy.
- Supports two source layouts in archive:
  - unity_plugin/BrightDataSDK
  - unity_editor_sample_app/Assets/BrightDataSDK
- Removes asmdef files from extracted temporary source.
- Copies to:
  - Assets/Plugins/Apple/BrightSDK
- Sets PluginImporter compatibility:
  - iOS true, tvOS false, Android false, StandaloneOSX false

### Apple Desktop (macOS)

Extractor: AppleDesktopBrightSDKExtractor

- Unzips and copies to:
  - Assets/Plugins/Apple/BrightSDK-macOS
- Removes extracted Editor directory before final copy.
- Sets PluginImporter compatibility:
  - StandaloneOSX true, others false

### Windows

Extractor: WindowsBrightSDKExtractor

- Extracts Windows SDK package into:
  - Assets/Plugins/Windows/BrightSDK
- Performs cleanup of obsolete files before replacement.
- Uses P/Invoke bridge at runtime to native `lum_sdk32` / `lum_sdk64`.

---

## Scripting Define Management (Apple)

Source: Editor/AppleBrightSDKDefinerPrebuild.cs

AssetPostprocessor updates the `APPLE_BRIGHT_SDK` define symbol automatically:

- Enabled for iOS/tvOS when Apple mobile framework exists.
- Enabled for Standalone when macOS artifacts exist.
- Removed when those plugin artifacts are absent.

This keeps Apple helper code gated by actual installed SDK assets.

---

## Runtime API Helpers (Scene Integration)

Base class:

- Scripts/BrightSdkHelper.cs

Features:

- UnityEvent<bool> onStatusChangeCallback
- Configurable consent text fields:
  - benefit
  - agreeBtn
  - disagreeBtn
- skipConsent bool
- Thread-safe status handoff to main Unity Update loop

### Android helper

File: Scripts/AndroidBrightSDKHelper.cs

- Uses AndroidJavaObject bridge to `com.android.eapx.BrightApi`.
- Builds `com.android.eapx.Settings` with consent text + skipConsent.
- Registers status callback via AndroidJavaProxy implementing `Settings$OnStatusChange`.
- Exposes methods:
  - ShowConsent
  - ExternalOptIn
  - NotifyConsentShown
  - OptOut
  - IsEnabled

### Apple helper

File: Scripts/AppleBrightSDKHelper.cs

iOS/tvOS path:

- Uses BrdsdkBridge APIs:
  - tryInit(..., skipConsent)
  - show_consent
  - external_opt_in
  - notify_consent_shown
  - opt_out
  - current_choice
- Registers choice-change callback and forwards to UnityEvent.

macOS path:

- Uses BrdsdkBridgeMacOS instance.
- Registers choice-change callback.
- Exposes ExternalOptIn/NotifyConsentShown/ShowConsent/OptOut/IsEnabled.

### Windows helper

File: Scripts/WinBrightSdkHelper.cs

- Initializes bridge on Awake.
- Registers both choice and service-status callbacks.
- Calls `FixService()` if status indicates not installed/not running.
- Deinitializes on application quit.

Underlying native bridge:

- Scripts/BrdsdkBridgeWin.cs
- P/Invoke calls into `lum_sdk32` / `lum_sdk64`:
  - init, show consent, opt out, close, fix service status, choice callback, status callback

---

## Installation

From README:

1. Install dependency package:
   - com.unity.sharp-zip-lib (via Package Manager)
2. In Unity project root, run:

```bash
wget https://raw.githubusercontent.com/BrightSDK/unity-plugin/refs/heads/main/install.sh && chmod +x install.sh
```

3. Restart Unity project.

The plugin runs automatically during Build / Build and Run.

---

## Runtime Integration Pattern

Attach one of these helper MonoBehaviours in scene depending on target platform:

- AndroidBrightSDKHelper
- AppleBrightSDKHelper
- WinBrightSDKHelper

Then wire:

- UI actions to ShowConsent / OptOut / ExternalOptIn / NotifyConsentShown
- onStatusChangeCallback for app logic reacting to consent state

---

## Windows Config Note

README requires `win_brd_config.json` for Windows SDK integration and references `brd_config.json` format from Windows SDK package.

Required fields:

- app_id
- app_name
- logo_link

---

## Package Metadata

From package.json:

- unity: 2019.4 (declared)
- dependency: com.unity.sharp-zip-lib 1.3.9
- description: prebuild updater
- npm metadata used for Unity package distribution

README tested requirement note mentions Unity 2022.3+.

---

## Notable Limitations

From README and source behavior:

- Not intended to run in Unity Editor play mode.
- macOS app bundle build from Editor is listed as not supported (Xcode workflow).
- Platform behavior depends on native SDK artifacts being present in Plugins directories.

---

## Source Map

Key files:

- README.md
- Editor/BrightSDKLoaderPrebuild.cs
- Editor/AppleBrightSDKDefinerPrebuild.cs
- Editor/Utils/BrightSDKArchiveDownloader.cs
- Editor/Utils/BrightSDKExtractor.cs
- Editor/Utils/BrightSDKVersions.cs
- Editor/Utils/BrightSDKConfig.cs
- Editor/BrightSDK.json
- Scripts/BrightSdkHelper.cs
- Scripts/AndroidBrightSDKHelper.cs
- Scripts/AppleBrightSDKHelper.cs
- Scripts/WinBrightSdkHelper.cs
- Scripts/BrdsdkBridgeWin.cs
