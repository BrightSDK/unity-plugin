using System;
using System.IO;
using System.Text;
using System.Linq;
using System.IO.Compression;
using Unity.SharpZipLib.Tar;
using Unity.SharpZipLib.GZip;
using UnityEngine;
using UnityEditor;

interface BrightSDKExtractor
{
    public void Extract(string sourceFile);
}

class AndroidBrightSDKExtractor : BrightSDKExtractor
{
    private string sdkDir;

    public AndroidBrightSDKExtractor()
    {
        sdkDir = BrightSDKDirectory.PluginsDir("Android");
    }

    public void Extract(string sourceFile)
    {
        RemoveObsoleteFiles();
        ExtractBrightSdk(sourceFile);
    }

    private void RemoveObsoleteFiles()
    {
        // Remove obsolete bright_sdk*.aar files
        Debug.Log("AndroidBrightSDKExtractor: Removing obsolete bright_sdk*.aar files");
        string[] obsoleteAarFiles = Directory.GetFiles(sdkDir, "bright_sdk*.aar", SearchOption.TopDirectoryOnly);
        foreach (string file in obsoleteAarFiles)
        {
            Debug.Log($"AndroidBrightSDKExtractor: Deleting obsolete AAR file {file}");
            File.Delete(file);
        }
    }

    private void ExtractBrightSdk(string sourceFile)
    {
        Debug.Log("AndroidBrightSDKExtractor: Extracting Bright SDK");
        string extractDir = Path.Combine(BrightSDKDirectory.CacheDir, "extracted/Android");
        if (Directory.Exists(extractDir))
            Directory.Delete(extractDir, true);
        Directory.CreateDirectory(extractDir);

        unzip(sourceFile, extractDir);

        // Find the .aar file recursively
        string aarFile = Directory.GetFiles(extractDir, "*.aar", SearchOption.AllDirectories).FirstOrDefault();
        string destAarFile = Path.Combine(sdkDir, "bright_sdk.aar");

        if (File.Exists(destAarFile))
            File.Delete(destAarFile);

        if (aarFile != null && File.Exists(aarFile))
        {
            File.Copy(aarFile, destAarFile);
            AssetDatabase.Refresh();
            Debug.Log($"AndroidBrightSDKExtractor: AAR file found and copied from {aarFile} to {destAarFile}");
        }
        else
            Debug.LogError($"AndroidBrightSDKExtractor: AAR file not found in {extractDir}");
    }

    private void unzip(string sourceFile, string extractDir)
    {
        using (FileStream fs = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
        using (GZipInputStream gzipStream = new GZipInputStream(fs))
        using (TarInputStream tarStream = new TarInputStream(gzipStream, Encoding.UTF8))
        {
            TarEntry entry;
            while ((entry = tarStream.GetNextEntry()) != null)
            {
                string name = entry.Name;
                string outputPath = Path.Combine(extractDir, name);

                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(outputPath);
                }
                else
                {
                    string directoryPath = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        tarStream.CopyEntryContents(outputStream);
                    }
                }
            }
        }
    }
}

class AppleBrightSDKExtractor : BrightSDKExtractor
{
    private string relativeSdkPath;
    private string sdkDir;

    public AppleBrightSDKExtractor()
    {
        relativeSdkPath = "Apple/BrightDataSDK";
        sdkDir = BrightSDKDirectory.PluginsDir(relativeSdkPath);
    }

    public void Extract(string sourceFile)
    {
        RemoveObsoleteFiles();
        ExtractBrightSdk(sourceFile);
    }

    private void RemoveObsoleteFiles()
    {
        Debug.Log("AppleBrightSDKExtractor: Removing obsolete SDK's files");
        foreach (string file in Directory.GetFiles(sdkDir, "*.*"))
        {
            Debug.Log($"AppleBrightSDKExtractor: Deleting file {file}");
            File.Delete(file);
        }
        foreach (string dir in Directory.GetDirectories(sdkDir))
        {
            Debug.Log($"AppleBrightSDKExtractor: Deleting folder {dir}");
            Directory.Delete(dir, true);
        }
    }

    private void ExtractBrightSdk(string sourceFile)
    {
        Debug.Log("AppleBrightSDKExtractor: Extracting Bright SDK");
        string extractDir = Path.Combine(BrightSDKDirectory.CacheDir, "extracted", relativeSdkPath);
        if (Directory.Exists(extractDir))
            Directory.Delete(extractDir, true);
        Directory.CreateDirectory(extractDir);

        ZipFile.ExtractToDirectory(sourceFile, extractDir);

        string destDir = sdkDir;
        if (Directory.Exists(destDir))
            Directory.Delete(destDir, true);

        string srcDir = Path.Combine(extractDir, "unity_plugin/BrightDataSDK");
        if (!Directory.Exists(srcDir))
            srcDir = Path.Combine(extractDir, "unity_editor_sample_app/Assets/BrightDataSDK");
        string asmDefFile = Path.Combine(srcDir, "AppleBrightSDK.asmdef");
        if (File.Exists(asmDefFile))
            File.Delete(asmDefFile);
        asmDefFile = Path.Combine(srcDir, "AppleBrightSDK.asmdef.meta");
        if (File.Exists(asmDefFile))
            File.Delete(asmDefFile);
        BrightSDKDirectory.CopyDirectory(srcDir, destDir, true);
        setSettingsOfFramework(destDir);
        AssetDatabase.Refresh();
        Debug.Log("AppleBrightSDKExtractor: Bright SDK files copied");
    }

    private void setSettingsOfFramework(string frameworkRoot)
    {
        Debug.Log("AppleBrightSDKExtractor: Set settings for framework");
        string frameworkPath = Path.Combine(frameworkRoot, "brdsdk.framework");
        PluginImporter plugin = AssetImporter.GetAtPath(frameworkPath) as PluginImporter;
        if (plugin == null)
            return;
        plugin.SetCompatibleWithAnyPlatform(false);
        plugin.SetCompatibleWithEditor(false);
        plugin.SetCompatibleWithPlatform(BuildTarget.iOS, true);
        plugin.SetCompatibleWithPlatform(BuildTarget.tvOS, false);
        plugin.SetCompatibleWithPlatform(BuildTarget.Android, false);
        plugin.SaveAndReimport();
    }
}