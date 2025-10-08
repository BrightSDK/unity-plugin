using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AppleBrightSDKDefiner: AssetPostprocessor
{
    const string Define = "APPLE_BRIGHT_SDK";

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        Apply();
    }

    public static void Apply()
    {
        updateForPlatform(BuildTargetGroup.iOS);
        updateForPlatform(BuildTargetGroup.tvOS);
    }

    private static void updateForPlatform(BuildTargetGroup group)
    {
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

        bool pluginExists = frameworkExists();
        bool defineExists = defines.Contains(Define);

        if (pluginExists && !defineExists)
        {
            defines += ";" + Define;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
        }
        else if (!pluginExists && defineExists)
        {
            defines = defines.Replace(Define, "").Replace(";;", ";").Trim(';');
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
        }
    }

    private static bool frameworkExists()
    {
        string path = "Assets/Plugins/Apple/BrightDataSDK/brdsdk.xcframework";
        if (Directory.Exists(path))
            return true;
        path = "Assets/Plugins/Apple/BrightDataSDK/brdsdk.framework";
        if (Directory.Exists(path))
            return true;
        return false;
    }
}