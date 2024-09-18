using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class InstallDependencies : AssetPostprocessor
{
    private static ListRequest listRequest;
    private static AddRequest addRequest;
    private const string dependencyName = "com.unity.sharp-zip-lib";
    private const string dependencyVersion = "1.3.8";

    static InstallDependencies()
    {
        // Check for the dependency after asset import
        CheckAndInstallDependency();
    }

    private static void CheckAndInstallDependency()
    {
        listRequest = Client.List();
        EditorApplication.update += ListDependencies;
    }

    private static void ListDependencies()
    {
        if (listRequest.IsCompleted)
        {
            if (listRequest.Status == StatusCode.Success)
            {
                bool isDependencyInstalled = false;

                foreach (var package in listRequest.Result)
                {
                    if (package.name == dependencyName && package.version == dependencyVersion)
                    {
                        isDependencyInstalled = true;
                        break;
                    }
                }

                if (!isDependencyInstalled)
                {
                    Debug.Log("InstallDependencies: Dependency not installed, installing...");
                    AddDependency();
                }
                else
                {
                    Debug.Log("InstallDependencies: Dependency already installed.");
                }
            }
            else
            {
                Debug.LogError("InstallDependencies: Error fetching package list: " + listRequest.Error.message);
            }

            // Remove the update callback
            EditorApplication.update -= ListDependencies;
        }
    }

    private static void AddDependency()
    {
        addRequest = Client.Add(dependencyName + "@" + dependencyVersion);
        EditorApplication.update += AddDependencyProgress;
    }

    private static void AddDependencyProgress()
    {
        if (addRequest.IsCompleted)
        {
            if (addRequest.Status == StatusCode.Success)
            {
                Debug.Log("InstallDependencies: Dependency installed successfully.");
            }
            else
            {
                Debug.LogError("InstallDependencies: Error installing dependency: " + addRequest.Error.message);
            }

            // Remove the update callback
            EditorApplication.update -= AddDependencyProgress;
        }
    }
}
