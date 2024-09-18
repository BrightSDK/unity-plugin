# Bright SDK Unity Plugin

## Overview

The **Bright SDK Unity Plugin** is a Unity Editor extension designed to automate the process of integrating the Bright SDK into your Unity Android projects. This plugin handles downloading, updating, and extracting the Bright SDK, ensuring you have the latest version integrated into your project seamlessly.

## Features

- **Automatic SDK Updates:** Automatically fetches and updates the Bright SDK before building your Android project.
- **Version Management:** Supports fetching the latest SDK versions from the server.
- **Clean Up:** Removes obsolete SDK files to prevent conflicts and ensure a clean build environment.
- **Extraction:** Extracts the Bright SDK from a compressed file and places it in the appropriate directory.


## Installation

### Install depencendies

1. Open Unity and go to `Window > Package Manager`.
2. Click the `+` button in the top left corner.
3. Select `Add package by name...`.
4. Enter `com.unity.sharp-zip-lib` as package name.
5. Click `Add`.

### Add plugin to your project

1. Open Unity and go to `Window > Package Manager`.
2. Click the `+` button in the top left corner.
3. Select `Add package from git URL...`.
4. Enter `git@github.com:vladislavs-luminati/bright-sdk-unity-updater.git`.
5. Click `Add`.

This will add your package to the Unity project, and the `BrightSdkPreBuildProcessor` script will be available for use.

## Usage

The plugin runs automatically during the build process for Android projects. 

1. Open the **Build Settings** in Unity (File > Build Settings).
2. Select **Android** as the platform.
3. Click on **Build** or **Build and Run**.
4. The plugin will execute the `OnPreprocessBuild` method, which includes:
   - Fetching the latest Bright SDK versions.
   - Downloading and extracting the latest SDK if necessary.
   - Cleaning up obsolete SDK files.

## Customization

You can customize the SDK version by modifying the `sdkVersion` variable in the `BrightSdkPreBuildProcessor` class. The plugin currently defaults to the latest version.

## Debugging

The plugin provides debug logs in the Unity Console, allowing you to track the process and identify any issues during the SDK update process.

## Requirements

- Unity 2019.4 or later
- Android Build Support installed in Unity

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.

## Contributing

Contributions are welcome! Please fork this repository and submit a pull request if you would like to add features or improve the plugin.

## Support

For issues or support, please open an issue in this repository.
