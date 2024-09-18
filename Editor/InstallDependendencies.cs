using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class InstallDependencies
{
    private static AddRequest Request;

    [InitializeOnLoadMethod]
    private static void AddSharpZipLibPackage()
    {
        Debug.Log("AddSharpZipLibPackage method called.");

        // Check if the package is already installed
        if (!IsPackageInstalled("com.unity.sharp-zip-lib"))
        {
            Debug.Log("Adding com.unity.sharp-zip-lib package...");
            Request = Client.Add("com.unity.sharp-zip-lib@1.3.8");
        }
        else
        {
            Debug.Log("Package com.unity.sharp-zip-lib is already installed.");
        }
    }

    private static bool IsPackageInstalled(string packageName)
    {
        // Placeholder implementation
        Debug.Log($"Checking if package {packageName} is installed.");
        // Implement the actual check here
        return false;
    }
}