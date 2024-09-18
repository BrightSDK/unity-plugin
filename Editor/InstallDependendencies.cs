using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class InstallDependencies
{
    private static AddRequest addRequest;
    private const string dependencyName = "com.unity.sharp-zip-lib";
    private const string dependencyVersion = "1.3.8";

    [InitializeOnLoadMethod]
    private static void InstallDependencyOnLoad()
    {
        // Check if the dependency is already installed
        ListRequest listRequest = Client.List();
        EditorApplication.update += () => CheckDependency(listRequest);
    }

    private static void CheckDependency(ListRequest listRequest)
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
                    Debug.Log("BrightSdkUpdater DepencencyInstaller: Dependency not installed, installing...");
                    AddDependency();
                }
                else
                {
                    Debug.Log("BrightSdkUpdater DepencencyInstaller: Dependency already installed.");
                }
            }
            else
            {
                Debug.LogError("BrightSdkUpdater DepencencyInstaller: Error fetching package list: " + listRequest.Error.message);
            }

            // Remove the update callback
            EditorApplication.update -= () => CheckDependency(listRequest);
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
                Debug.Log("BrightSdkUpdater DepencencyInstaller: Dependency installed successfully.");
            }
            else
            {
                Debug.LogError("BrightSdkUpdater DepencencyInstaller: Error installing dependency: " + addRequest.Error.message);
            }

            // Remove the update callback
            EditorApplication.update -= AddDependencyProgress;
        }
    }
}
