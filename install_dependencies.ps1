$MANIFEST_FILE = "Packages\manifest.json"
$PACKAGE_NAME = "com.unity.sharp-zip-lib"
$PACKAGE_VERSION = "1.3.9"

if (-not (Test-Path $MANIFEST_FILE)) {
    Write-Host "manifest.json file not found!"
    exit 1
}

$content = Get-Content $MANIFEST_FILE -Raw
if ($content -match [regex]::Escape("`"$PACKAGE_NAME`"")) {
    Write-Host "SharpZipLib package already exists in manifest.json"
} else {
    $json = $content | ConvertFrom-Json
    $json.dependencies | Add-Member -NotePropertyName $PACKAGE_NAME -NotePropertyValue $PACKAGE_VERSION -Force
    $json | ConvertTo-Json -Depth 10 | Set-Content $MANIFEST_FILE
    Write-Host "SharpZipLib package added to manifest.json"
}
