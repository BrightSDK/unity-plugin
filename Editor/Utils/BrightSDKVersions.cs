using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

class BrightSDKVersions
{
    private const string IntegrationConfigUrl =
        "https://bright-sdk.com/sdk_api/sdk/integration/config";

    private Dictionary<string, string> lastVersions;

    public void load()
    {
        Debug.Log("BrightSDKVersions: Fetching Bright SDK versions");
        lastVersions = loadViaFFI();
        if (lastVersions == null)
        {
            Debug.Log("BrightSDKVersions: FFI unavailable, falling back to HTTP");
            string json = getIntegrationConfigContent();
            lastVersions = parseVersions(json);
        }
        Debug.Log($"BrightSDKVersions: Loaded SDK versions: android={lastVersions.GetValueOrDefault("android")}, ios={lastVersions.GetValueOrDefault("ios")}, macos={lastVersions.GetValueOrDefault("macos")}, win={lastVersions.GetValueOrDefault("win")}");
    }

    public string LastVersion(BuildTarget platform)
    {
        if (lastVersions == null) return null;
        if (platform == BuildTarget.iOS || platform == BuildTarget.tvOS)
            return lastVersions.GetValueOrDefault("ios");
        else if (platform == BuildTarget.StandaloneOSX)
            return lastVersions.GetValueOrDefault("macos");
        else if (platform == BuildTarget.Android)
            return lastVersions.GetValueOrDefault("android");
        else if (platform == BuildTarget.StandaloneWindows || platform == BuildTarget.StandaloneWindows64)
            return lastVersions.GetValueOrDefault("win");

        return null;
    }

    private Dictionary<string, string> loadViaFFI()
    {
        try
        {
            BrightSdkDownloaderFFI.EnsureLoaded();
            var result = new Dictionary<string, string>();
            foreach (var kv in new[] {
                new[] { "android", "android" },
                new[] { "ios", "ios" },
                new[] { "macos", "macos" },
                new[] { "win", "win" }
            })
            {
                string json = BrightSdkDownloaderFFI.Resolve(kv[1], "latest");
                if (json == null) continue;
                string ver = extractJsonField(json, "version");
                if (!string.IsNullOrEmpty(ver))
                    result[kv[0]] = ver;
            }
            if (result.Count > 0)
            {
                Debug.Log("BrightSDKVersions: Resolved versions via downloader-rs FFI");
                return result;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"BrightSDKVersions: FFI load failed: {e.Message}");
        }
        return null;
    }

    private static string extractJsonField(string json, string field)
    {
        var match = Regex.Match(json, "\"" + field + "\"\\s*:\\s*\"([^\"]+)\"");
        return match.Success ? match.Groups[1].Value : null;
    }

    private string getIntegrationConfigContent()
    {
        string cacheFile = Path.Combine(BrightSDKDirectory.CacheDir, "sdk_integration_config.json");

        if (File.Exists(cacheFile))
        {
            FileInfo fileInfo = new FileInfo(cacheFile);
            if (fileInfo.LastWriteTime < DateTime.Now.AddDays(-1))
                File.Delete(cacheFile);
        }

        if (!File.Exists(cacheFile))
        {
            string apiKey = Environment.GetEnvironmentVariable("SDK_API_KEY");
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(IntegrationConfigUrl);
            if (!string.IsNullOrEmpty(apiKey))
                req.Headers.Add("api-key", apiKey);
            req.Timeout = 10000;
            using (HttpWebResponse resp = (HttpWebResponse) req.GetResponse())
            using (StreamReader reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                Directory.CreateDirectory(BrightSDKDirectory.CacheDir);
                File.WriteAllText(cacheFile, json);
            }
        }

        return File.ReadAllText(cacheFile);
    }

    private Dictionary<string, string> parseVersions(string json)
    {
        var result = new Dictionary<string, string>();
        foreach (string platform in new[] { "android", "ios", "macos", "win" })
        {
            var match = Regex.Match(json,
                "\"" + platform + "\"\\s*:\\s*\\{[^}]*?\"last_version\"\\s*:\\s*\"([^\"]+)\"",
                RegexOptions.Singleline);
            if (match.Success)
                result[platform] = match.Groups[1].Value;
        }
        return result;
    }
}
