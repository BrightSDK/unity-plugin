using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// P/Invoke bridge to the bright-sdk-downloader-rs native library.
/// Auto-downloads the platform-appropriate shared library on first use.
/// </summary>
static class BrightSdkDownloaderFFI
{
    private const string LibName = "bright_sdk_download";
    private const string DownloaderVersion = "1.0.0";
    private const string ReleaseBase =
        "https://github.com/BrightSDK/bright-sdk-downloader-rs/releases/download/"
        + DownloaderVersion + "/";

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr sdk_resolve(string platform, string version);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr sdk_fetch(string platform, string version, string outputDir);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr sdk_list_platforms();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr sdk_last_error();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void sdk_free_string(IntPtr ptr);

    private static bool initialized;

    /// <summary>
    /// Ensures the native library is available, downloading it if needed.
    /// </summary>
    public static void EnsureLoaded()
    {
        if (initialized) return;
        string libPath = GetNativeLibPath();
        if (!File.Exists(libPath))
        {
            Debug.Log("BrightSdkDownloaderFFI: Downloading native library...");
            DownloadNativeLib(libPath);
        }
        initialized = true;
    }

    /// <summary>
    /// Resolve latest version and download URL for a platform.
    /// Returns JSON: {platform, version, url} or null on error.
    /// </summary>
    public static string Resolve(string platform, string version = "latest")
    {
        EnsureLoaded();
        IntPtr ptr = sdk_resolve(platform, version);
        return PtrToStringAndFree(ptr);
    }

    /// <summary>
    /// Download and extract SDK archive for a platform.
    /// Returns JSON: {platform, version, url, output} or null on error.
    /// </summary>
    public static string Fetch(string platform, string version, string outputDir)
    {
        EnsureLoaded();
        IntPtr ptr = sdk_fetch(platform, version, outputDir);
        return PtrToStringAndFree(ptr);
    }

    /// <summary>
    /// List all available platforms.
    /// Returns JSON array or null on error.
    /// </summary>
    public static string ListPlatforms()
    {
        EnsureLoaded();
        IntPtr ptr = sdk_list_platforms();
        return PtrToStringAndFree(ptr);
    }

    /// <summary>
    /// Get last error message from the native library.
    /// </summary>
    public static string LastError()
    {
        IntPtr ptr = sdk_last_error();
        return PtrToStringAndFree(ptr);
    }

    private static string PtrToStringAndFree(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero) return null;
        string result = Marshal.PtrToStringAnsi(ptr);
        sdk_free_string(ptr);
        return result;
    }

    private static string GetNativeLibPath()
    {
        string cacheDir = BrightSDKDirectory.CacheDir;
        string libFileName = GetNativeLibFileName();
        return Path.Combine(cacheDir, libFileName);
    }

    private static string GetNativeLibFileName()
    {
#if UNITY_EDITOR_WIN
        return "bright_sdk_download.dll";
#elif UNITY_EDITOR_OSX
        return "libbright_sdk_download.dylib";
#else
        return "libbright_sdk_download.so";
#endif
    }

    private static void DownloadNativeLib(string destPath)
    {
        string fileName = GetNativeLibFileName();
        string url = ReleaseBase + fileName;
        Debug.Log($"BrightSdkDownloaderFFI: Downloading from {url}");
        string dir = Path.GetDirectoryName(destPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        using (WebClient client = new WebClient())
        {
            client.DownloadFile(url, destPath);
        }
        // Add the cache dir to the native library search path
        string currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        if (!currentPath.Contains(dir))
        {
            Environment.SetEnvironmentVariable("PATH", dir + Path.PathSeparator + currentPath);
        }
        Debug.Log($"BrightSdkDownloaderFFI: Saved to {destPath}");
    }
}
