# Bright SDK Unity Plugin

## ⚠️ Breaking change in v1.1.0 — API key now required

Starting with **v1.1.0** the plugin authenticates with the BrightSDK releases API.
**Older versions will stop working once the unauthenticated endpoint is retired.**

**What you need to do:**

1. Upgrade to the latest plugin version (v1.1.0+)
2. Obtain a BrightSDK API key — see [How to obtain an API key](docs/obtain-api-key.md)
3. Export the key before triggering a Unity build:
   ```bash
   export SDK_API_KEY=<your-api-key>
   ```

## Overview

The **Bright SDK Unity Plugin** is a Unity Editor extension designed to automate the process of integrating the Bright SDK into your Unity projects for next platforms

- Android
- iOS/tvOS (Apple Mobile)
- macOS (Apple Desktop)
- Windows

This plugin handles downloading, updating, and extracting the Bright SDK, ensuring you have the latest version integrated into your project seamlessly.

## Features

- **Automatic SDK Updates:** Automatically fetches and updates the Bright SDK before building your Android project.
- **Version Management:** Supports fetching the latest SDK versions from the server.
- **Clean Up:** Removes obsolete SDK files to prevent conflicts and ensure a clean build environment.
- **Extraction:** Extracts the Bright SDK from a compressed file and places it in the appropriate directory.


## Installation

### Add plugin [SharpZipLib](https://docs.unity3d.com/Packages/com.unity.sharp-zip-lib@1.3/manual/Installation.html)

1. Select `Window > Package Manager`.
2. Click the `+` button in the top left corner.
3. Select `Add package by name...`.
4. Enter `com.unity.sharp-zip-lib` as package name.
5. Click `Add`.

### Install SDK Unity Plugin

#### macOS / Linux

1. Open Terminal.app
2. `cd` to your Unity project directory
3. Run:

   ```bash
   wget https://raw.githubusercontent.com/BrightSDK/unity-plugin/refs/heads/main/install_dependencies.sh -O install_dependencies.sh && chmod +x install_dependencies.sh && ./install_dependencies.sh && rm install_dependencies.sh
   wget https://raw.githubusercontent.com/BrightSDK/unity-plugin/refs/heads/main/install.sh -O install.sh && chmod +x install.sh && ./install.sh && rm install.sh
   ```

#### Windows (PowerShell)

1. Open PowerShell
2. `cd` to your Unity project directory
3. Run:

   ```powershell
   Invoke-WebRequest https://raw.githubusercontent.com/BrightSDK/unity-plugin/refs/heads/main/install_dependencies.ps1 -OutFile install_dependencies.ps1; .\install_dependencies.ps1; Remove-Item install_dependencies.ps1
   Invoke-WebRequest https://raw.githubusercontent.com/BrightSDK/unity-plugin/refs/heads/main/install.ps1 -OutFile install.ps1; .\install.ps1; Remove-Item install.ps1
   ```

   > If you see an execution policy error, run `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass` first.

### Set your API key

The plugin authenticates with the BrightSDK releases API using an API key read from the `SDK_API_KEY` environment variable. Export it before triggering a build:

```bash
export SDK_API_KEY=my-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

For how to obtain a key, see [docs/obtain-api-key.md](docs/obtain-api-key.md).

### Restart Unity
	
1. Select `Unity -> Quit`
1. Open Unity project again from Unity Hub

## Usage

The plugin runs automatically during the build process. 

1. Open the **Build Settings** in Unity (File > Build Settings).
2. Select platform.
3. Click on **Build** or **Build and Run**.

**Not supported**

- Running in Editor.
- Building macOS app bundle from Editor (only through Xcode project).

### Integration in scene

Under `Assets/Scripts/BrightSDK` folder you can find `AndroidBrightSDKHelper`, `AppleBrightSDKHelper` and `WinBrightSDKHelper` files which wrap SDK APIs of relative systems and are assignable to your scene object and its actions.

## Customization

You can set version of SDK in config file `Assets/Editor/BrightSDK/BrightSDK.json`. **null** value means the latest version. It supports next properties:

- **android** for Android SDK version
- **appleMobile** for iOS and tvOS
- **appleDesktop** for macOS
- **windows** for Windows

By using SDK helpers above you can assign texts for SDK consent screen buttons and benefit message, and subscribe on choice-change events.

For Windows SDK you should create a file with name `win_brd_config.json` and fill necessary SDK properties. For the format of content you can check `brd_config.json` from Windows SDK plugin.

Required properties are

- **app_id**
- **app_name**
- **logo_link**

## Debugging

The plugin provides debug logs in the Unity Console, allowing you to track the process and identify any issues during the SDK update process.

## Requirements

- Unity 2022.3 or later (other versions can also be supported, but were not tested, feel free to report)
- Android Build Support installed in Unity
- `SDK_API_KEY` environment variable set to your BrightSDK API key (see [obtain-api-key.md](docs/obtain-api-key.md))

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.md) file for more information.

## Contributing

Contributions are welcome! Please fork this repository and submit a pull request if you would like to add features or improve the plugin.

## Support

For issues or support, please open an issue in this repository.
