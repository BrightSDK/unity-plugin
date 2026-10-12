using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using Unity.SharpZipLib.GZip;
using Unity.SharpZipLib.Tar;

[TestFixture]
public class BrightSDKSafetyTests
{
    private string tempDir;

    [SetUp]
    public void SetUp()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "BrightSDK_SecurityTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(tempDir))
        {
            try { Directory.Delete(tempDir, true); } catch {}
        }
    }

    [Test]
    public void Extractor_RejectsTarPathTraversal()
    {
        string archiveFile = Path.Combine(tempDir, "evil_relative.tar.gz");
        CreateTarGz(archiveFile, "../../pwn.txt", Encoding.UTF8.GetBytes("pwned"));

        var extractor = new AndroidBrightSDKExtractor();
        Assert.Throws<IOException>(() => extractor.Extract(archiveFile));
    }

    [Test]
    public void Extractor_RejectsRootedTarEntry()
    {
        string archiveFile = Path.Combine(tempDir, "evil_rooted.tar.gz");
        CreateTarGz(archiveFile, "/tmp/pwn_rooted.txt", Encoding.UTF8.GetBytes("pwned"));

        var extractor = new AndroidBrightSDKExtractor();
        Assert.Throws<IOException>(() => extractor.Extract(archiveFile));
    }

    private static void CreateTarGz(string archivePath, string entryName, byte[] data)
    {
        using (var fs = new FileStream(archivePath, FileMode.Create, FileAccess.Write))
        using (var gz = new GZipOutputStream(fs))
        using (var tar = new TarOutputStream(gz, Encoding.UTF8))
        {
            var entry = TarEntry.CreateTarEntry(entryName);
            entry.Size = data.Length;
            tar.PutNextEntry(entry);
            tar.Write(data, 0, data.Length);
            tar.CloseEntry();
        }
    }
}
