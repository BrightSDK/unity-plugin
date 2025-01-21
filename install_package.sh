#!/bin/bash

# Add SharpZipLib package to manifest.json if it doesn't exist
MANIFEST_FILE="Packages/manifest.json"
PACKAGE_NAME="com.brightdata.brightsdkunityupdater"
PACKAGE_VERSION="https://github.com/BrightSDK/unity-plugin.git"

if [ ! -f "$MANIFEST_FILE" ]; then
  echo "manifest.json file not found!"
  exit 1
fi

# Check if the package already exists in the manifest.json
if grep -q "\"$PACKAGE_NAME\"" "$MANIFEST_FILE"; then
  echo "SharpZipLib package already exists in manifest.json"
else
  # Add the package to the manifest.json
  jq ".dependencies += {\"$PACKAGE_NAME\": \"$PACKAGE_VERSION\"}" "$MANIFEST_FILE" > "$MANIFEST_FILE.tmp" && mv "$MANIFEST_FILE.tmp" "$MANIFEST_FILE"
  echo "BrightSdkUpdate package added to manifest.json"
fi