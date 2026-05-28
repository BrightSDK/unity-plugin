param(
    [string]$Branch = "main"
)

$PACKAGE_URL = "https://github.com/BrightSDK/unity-plugin/archive/refs/heads/$Branch.zip"
$TEMP_DIR = "Temp\BrightSDKPackage"
$TARGET_EDITOR_DIR = "Assets\Editor\BrightSDK"
$TARGET_SCRIPTS_DIR = "Assets\Scripts\BrightSDK"

# Create temporary directory
New-Item -ItemType Directory -Force -Path $TEMP_DIR | Out-Null

# Download the package
Write-Host "Downloading Bright SDK package..."
Invoke-WebRequest -Uri $PACKAGE_URL -OutFile "$TEMP_DIR\package.zip"

# Extract the package
Write-Host "Extracting Bright SDK package..."
Expand-Archive -Path "$TEMP_DIR\package.zip" -DestinationPath $TEMP_DIR -Force

$SOURCE_EDITOR_DIR = "$TEMP_DIR\unity-plugin-$Branch\Editor"
$SOURCE_SCRIPTS_DIR = "$TEMP_DIR\unity-plugin-$Branch\Scripts"

# Copy the Editor folder to Assets
if (Test-Path $SOURCE_EDITOR_DIR) {
    Write-Host "Copying Editor folder to Assets..."
    if (Test-Path $TARGET_EDITOR_DIR) { Remove-Item -Recurse -Force $TARGET_EDITOR_DIR }
    New-Item -ItemType Directory -Force -Path $TARGET_EDITOR_DIR | Out-Null
    Copy-Item -Recurse "$SOURCE_EDITOR_DIR\*" $TARGET_EDITOR_DIR
} else {
    Write-Host "Editor folder not found in the package."
}

# Copy the Scripts folder to Assets
if (Test-Path $SOURCE_SCRIPTS_DIR) {
    Write-Host "Copying Scripts folder to Assets..."
    if (Test-Path $TARGET_SCRIPTS_DIR) { Remove-Item -Recurse -Force $TARGET_SCRIPTS_DIR }
    New-Item -ItemType Directory -Force -Path $TARGET_SCRIPTS_DIR | Out-Null
    Copy-Item -Recurse "$SOURCE_SCRIPTS_DIR\*" $TARGET_SCRIPTS_DIR
} else {
    Write-Host "Scripts folder not found in the package."
}

# Clean up
Write-Host "Cleaning up..."
Remove-Item -Recurse -Force $TEMP_DIR

Write-Host "Bright SDK Updater package downloaded and installed successfully."
