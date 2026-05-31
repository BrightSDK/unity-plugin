using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.Collections.Generic;

public class BrightSDKLoaderPrebuild : IPreprocessBuildWithReport
{
    private readonly Dictionary<BuildTarget, BrightSDKExtractor> extractors = new Dictionary<BuildTarget, BrightSDKExtractor>();
    private readonly Dictionary<BuildTarget, BrightSDKArchiveDownloader> archiveDownloaders = new Dictionary<BuildTarget, BrightSDKArchiveDownloader>();
    private BrightSDKVersions sdkVersions;

    public int callbackOrder => 0;

    public BrightSDKLoaderPrebuild()
    {
        sdkVersions = new BrightSDKVersions();

        extractors[BuildTarget.Android] = new AndroidBrightSDKExtractor();
        extractors[BuildTarget.iOS] = extractors[BuildTarget.tvOS] = new AppleMobileBrightSDKExtractor();
        extractors[BuildTarget.StandaloneOSX] = new AppleDesktopBrightSDKExtractor();
        extractors[BuildTarget.StandaloneWindows] = extractors[BuildTarget.StandaloneWindows64] = new WindowsBrightSDKExtractor();

        archiveDownloaders[BuildTarget.Android] = new AndroidSDKArchiveDownloader();
        archiveDownloaders[BuildTarget.iOS] = archiveDownloaders[BuildTarget.tvOS] = new AppleMobileSDKArchiveDownloader();
        archiveDownloaders[BuildTarget.StandaloneOSX] = new AppleDesktopSDKArchiveDownloader();
        archiveDownloaders[BuildTarget.StandaloneWindows] = archiveDownloaders[BuildTarget.StandaloneWindows64] = new WindowsSDKArchiveDownloader();
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("BrightSDKLoaderPrebuild: OnPreprocessBuild called");
        BuildTarget platform = report.summary.platform;
        if (isPlatformSupported(platform))
        {
            Debug.Log("BrightSDKLoaderPrebuild: Platform is " + platform + ", updating Bright SDK");
            UpdateBrightSdk(platform);
        }
        else
        {
            Debug.Log("BrightSDKLoaderPrebuild: Platform " + platform + " is not supported, skipping Bright SDK update");
        }
    }

    private bool isPlatformSupported(BuildTarget platform)
    {
        return archiveDownloaders.ContainsKey(platform) && extractors.ContainsKey(platform);
    }

    private static readonly Dictionary<BuildTarget, string> ffiPlatformKeys =
        new Dictionary<BuildTarget, string>
    {
        { BuildTarget.Android, "android" },
        { BuildTarget.iOS, "ios" },
        { BuildTarget.tvOS, "ios" },
        { BuildTarget.StandaloneOSX, "macos" },
        { BuildTarget.StandaloneWindows, "win" },
        { BuildTarget.StandaloneWindows64, "win" },
    };

    private void UpdateBrightSdk(BuildTarget platform)
    {
        Debug.Log($"BrightSDKLoaderPrebuild: Starting Bright SDK update for platform {platform}");

        if (TryUpdateViaFFI(platform))
        {
            Debug.Log("BrightSDKLoaderPrebuild: Bright SDK updated via downloader-rs");
            return;
        }

        Debug.Log("BrightSDKLoaderPrebuild: FFI path unavailable, falling back to HTTP");
        sdkVersions.load();

        if (isPlatformSupported(platform) && sdkVersions.LastVersion(platform) != null)
        {
            string lastVersion = sdkVersions.LastVersion(platform);
            Debug.Log($"BrightSDKLoaderPrebuild: {platform} last version {lastVersion}");
            string archiveFile = archiveDownloaders[platform].Download(lastVersion);
            extractors[platform].Extract(archiveFile);
        }

        Debug.Log("BrightSDKLoaderPrebuild: Bright SDK updated successfully");
    }

    private bool TryUpdateViaFFI(BuildTarget platform)
    {
        if (!ffiPlatformKeys.ContainsKey(platform)) return false;
        string platformKey = ffiPlatformKeys[platform];
        try
        {
            BrightSdkDownloaderFFI.EnsureLoaded();
            string outputDir = BrightSDKDirectory.CacheDir;
            string result = BrightSdkDownloaderFFI.Fetch(platformKey, "latest", outputDir);
            if (result == null)
            {
                string err = BrightSdkDownloaderFFI.LastError();
                Debug.LogWarning($"BrightSDKLoaderPrebuild: FFI sdk_fetch returned null: {err}");
                return false;
            }
            Debug.Log($"BrightSDKLoaderPrebuild: FFI fetch result: {result}");
            // sdk_fetch downloads and extracts — the extractor still handles
            // placing files into the correct Unity directories
            string archivePath = extractFetchedArchivePath(result);
            if (archivePath != null && extractors.ContainsKey(platform))
                extractors[platform].Extract(archivePath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"BrightSDKLoaderPrebuild: FFI failed: {e.Message}");
            return false;
        }
    }

    private string extractFetchedArchivePath(string json)
    {
        // Parse "output" field from the JSON response
        var match = System.Text.RegularExpressions.Regex.Match(json,
            "\"output\"\\s*:\\s*\"([^\"]+)\"");
        return match.Success ? match.Groups[1].Value : null;
    }
}