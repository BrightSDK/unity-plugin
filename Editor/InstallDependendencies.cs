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
            EditorApplication.update += Progress;
        }
        else
        {
            Debug.Log("com.unity.sharp-zip-lib package is already installed.");
        }
    }

    private static void Progress()
    {
        if (Request.IsCompleted)
        {
            if (Request.Status == StatusCode.Success)
                Debug.Log("Installed: " + Request.Result.packageId);
            else if (Request.Status >= StatusCode.Failure)
                Debug.Log(Request.Error.message);

            EditorApplication.update -= Progress;
        }
    }

    private static bool IsPackageInstalled(string packageName)
    {
        var listRequest = Client.List(true);
        while (!listRequest.IsCompleted) { }

        if (listRequest.Status == StatusCode.Success)
        {
            foreach (var package in listRequest.Result)
            {
                if (package.name == packageName)
                {
                    return true;
                }
            }
        }
        return false;
    }
}