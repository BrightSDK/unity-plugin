#!/bin/bash

# URL to download the ZIP file
PACKAGE_URL="https://github.com/BrightSDK/unity-plugin/archive/refs/heads/main.zip"
TEMP_DIR="Temp/BrightSDKPackage"
PACKAGE_DIR="Assets/BrightSDKPackage"
TARGET_EDITOR_DIR="Assets/Editor/BrightSDK"
TARGET_SCRIPTS_DIR="Assets/Scripts/BrightSDK"

# Create temporary directory
mkdir -p $TEMP_DIR

# Download the package
echo "Downloading Bright SDK package..."
curl -L $PACKAGE_URL -o $TEMP_DIR/package.zip

# Extract the package
echo "Extracting Bright SDK package..."
unzip -q $TEMP_DIR/package.zip -d $TEMP_DIR

# Copy the Editor folder to Assets
SOURCE_EDITOR_DIR="$TEMP_DIR/unity-plugin-main/Editor"

if [ -d "$SOURCE_EDITOR_DIR" ]; then
    echo "Copying Editor folder to Assets..."
    rm -rf $TARGET_EDITOR_DIR
    mkdir -p $TARGET_EDITOR_DIR
    cp -r $SOURCE_EDITOR_DIR/* $TARGET_EDITOR_DIR/
else
    echo "Editor folder not found in the package."
fi

# Copy the Scripts folder to Assets
SOURCE_SCRIPTS_DIR="$TEMP_DIR/unity-plugin-main/Scripts"

if [ -d "$SOURCE_SCRIPTS_DIR" ]; then
    echo "Copying Scripts folder to Assets..."
    rm -rf $TARGET_SCRIPTS_DIR
    mkdir -p $TARGET_SCRIPTS_DIR
    cp -r $SOURCE_SCRIPTS_DIR/* $TARGET_SCRIPTS_DIR/
else
    echo "Scripts folder not found in the package."
fi

# Clean up
echo "Cleaning up..."
rm -rf $TEMP_DIR

echo "Bright SDK Updater package downloaded and installed successfully."