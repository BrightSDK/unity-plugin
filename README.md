# Bright SDK Unity Plugin

## Overview

The **Bright SDK Unity Plugin** is a Unity Editor extension designed to automate the process of integrating the Bright SDK into your Unity projects for next platforms

- Android
- iOS/tvOS (Apple Mobile)
- macOS (Apple Desktop)

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

1. Open Terminal.app
1. **cd** to your directory with unity project
1. Execute this comand

	```
	wget https://raw.githubusercontent.com/BrightSDK/unity-plugin/refs/heads/main/install.sh && chmod +x install.sh
	```

### Restart Unity
	
1. Select `Unity -> Quit`
1. Open Unity project again from Unity Hub

## Usage

The plugin runs automatically during the build process for Android or iOS projects. 

1. Open the **Build Settings** in Unity (File > Build Settings).
2. Select platform (Android or Apple's one).
3. Click on **Build** or **Build and Run**.
4. The plugin will execute the `OnPreprocessBuild` method, which includes:
   - Fetching the latest Bright SDK versions.
   - Downloading and extracting the latest SDK if necessary.
   - Preparing Xcode project in macOS case.
   - Cleaning up obsolete SDK files.

### Integration in scene

Under `Assets/Scripts/BrightSDK` folder you can find `AndroidBrightSDKHelper` and `AppleBrightSDKHelper` files for Android and Apple's platforms. They wrap SDK APIs of these systems and are assignable to your scene object and its actions.

## Customization

You can set version of SDK in config file `Assets/Editor/BrightSDK/BrightSDK.json`. **null** value means the latest version. It supports next properties:

- **android** for Android SDK version
- **appleMobile** for iOS and tvOS
- **appleDesktop** for macOS

By using `AndroidBrightSDKHelper` or `AppleBrightSDKHelper` you can set texts for SDK consent screen buttons and benefit text, and subscribe on choice-change events.

## Debugging

The plugin provides debug logs in the Unity Console, allowing you to track the process and identify any issues during the SDK update process.

## Requirements

- Unity 2022.3 or later (other versions can also be supported, but were not tested, feel free to report)
- Android Build Support installed in Unity

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.md) file for more information.

## Contributing

Contributions are welcome! Please fork this repository and submit a pull request if you would like to add features or improve the plugin.

## Support

For issues or support, please open an issue in this repository.
