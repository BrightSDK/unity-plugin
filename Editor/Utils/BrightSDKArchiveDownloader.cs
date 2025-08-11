using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEditor;

class BrightSDKArchiveDownloader
{
    private const string sdkUrl = "https://cdn.bright-sdk.com/static/";

    // null for latest
    public virtual string VersionsPlatformKey => null;

    public string Download(string lastVersion)
    {
        string configVersion = getConfigVersion();
        string remoteName = MakeRemoteFileName(configVersion, lastVersion);
        if (remoteName == null)
        {
            Debug.LogError("SDKArchiveDownloader: Unknown sdk remote file name.");
            return null;
        }
        string downloadURL = sdkUrl + remoteName;
        string targetFile = Path.Combine(BrightSDKDirectory.CacheDir, remoteName);
        downloadFile(downloadURL, targetFile);
        return targetFile;
    }

    public virtual string MakeRemoteFileName(string configVersion, string lastVersion)
    {
        return null;
    }

    private void downloadFile(string url, string targetFile)
    {
        if (!File.Exists(targetFile))
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, targetFile);
            }
        }
    }

    private string loadConfigVersionPath()
    {
        string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileName(path) == "BrightSDK.json")
                return path;
        }
        return null;
    }

    private string getConfigVersion()
    {
        string path = loadConfigVersionPath();
        if (string.IsNullOrWhiteSpace(path)) return null;
        string json = File.ReadAllText(path);
        BrightSDKConfig config = JsonUtility.FromJson<BrightSDKConfig>(json);
        string version = config.versions[VersionsPlatformKey];
        if (string.IsNullOrWhiteSpace(version)) return null;
        return version;
    }
}

class AndroidSDKArchiveDownloader : BrightSDKArchiveDownloader
{
    // null for latest
    public override string VersionsPlatformKey => "android";
    public override string MakeRemoteFileName(string configVersion, string lastVersion)
    {
        string version = configVersion ?? lastVersion;
        return "bright_sdk_android-" + version + ".tar.gz";
    }
}

class AppleSDKArchiveDownloader : BrightSDKArchiveDownloader
{
    // null for latest
    public override string VersionsPlatformKey => "apple";
    public override string MakeRemoteFileName(string configVersion, string lastVersion)
    {
        string version = configVersion ?? lastVersion;
        return "bright_sdk_ios-" + version + ".zip";
    }
}