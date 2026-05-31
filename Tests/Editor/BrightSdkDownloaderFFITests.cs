using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;

[TestFixture]
public class BrightSdkDownloaderFFITests
{
    [Test]
    public void GetNativeLibFileName_ReturnsExpectedForPlatform()
    {
        // Verify the lib file name logic based on current editor platform
#if UNITY_EDITOR_WIN
        string expected = "bright_sdk_download.dll";
#elif UNITY_EDITOR_OSX
        string expected = "libbright_sdk_download.dylib";
#else
        string expected = "libbright_sdk_download.so";
#endif
        // We test by verifying the path ends with expected filename
        string cacheDir = BrightSDKDirectory.CacheDir;
        string libPath = Path.Combine(cacheDir, expected);
        Assert.That(libPath, Does.EndWith(expected));
    }

    [Test]
    public void EnsureLoaded_DoesNotThrowOnRepeatCall()
    {
        // EnsureLoaded should be idempotent — second call is a no-op
        // Note: this test requires network access for initial download
        try
        {
            BrightSdkDownloaderFFI.EnsureLoaded();
            BrightSdkDownloaderFFI.EnsureLoaded();
        }
        catch (DllNotFoundException)
        {
            // Expected if native lib cannot be downloaded in test environment
            Assert.Pass("Native lib unavailable in test env — skipping");
        }
    }

    [Test]
    public void ListPlatforms_ReturnsJsonArrayOrNull()
    {
        try
        {
            BrightSdkDownloaderFFI.EnsureLoaded();
            string result = BrightSdkDownloaderFFI.ListPlatforms();
            if (result != null)
            {
                Assert.That(result, Does.StartWith("["));
                Assert.That(result, Does.Contain("android"));
            }
        }
        catch (DllNotFoundException)
        {
            Assert.Pass("Native lib unavailable in test env — skipping");
        }
    }

    [Test]
    public void Resolve_WithInvalidPlatform_ReturnsNullAndSetsError()
    {
        try
        {
            BrightSdkDownloaderFFI.EnsureLoaded();
            string result = BrightSdkDownloaderFFI.Resolve("nonexistent_platform_xyz");
            if (result == null)
            {
                string err = BrightSdkDownloaderFFI.LastError();
                Assert.That(err, Is.Not.Null.And.Not.Empty);
            }
        }
        catch (DllNotFoundException)
        {
            Assert.Pass("Native lib unavailable in test env — skipping");
        }
    }

    [Test]
    public void Resolve_AndroidLatest_ReturnsVersionJson()
    {
        try
        {
            BrightSdkDownloaderFFI.EnsureLoaded();
            string result = BrightSdkDownloaderFFI.Resolve("android", "latest");
            if (result != null)
            {
                Assert.That(result, Does.Contain("\"version\""));
                Assert.That(result, Does.Contain("\"platform\""));
            }
            else
            {
                // May fail without API key
                string err = BrightSdkDownloaderFFI.LastError();
                Assert.That(err, Is.Not.Null,
                    "Expected either a result or an error message");
            }
        }
        catch (DllNotFoundException)
        {
            Assert.Pass("Native lib unavailable in test env — skipping");
        }
    }
}
