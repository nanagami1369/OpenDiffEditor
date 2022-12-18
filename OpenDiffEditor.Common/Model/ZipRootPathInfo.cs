using System.IO;
using System.IO.Compression;

namespace OpenDiffEditor.Common.Model;

public class ZipRootPathInfo : IRootPathInfo
{
    public RootPathType Type { get; }
    public string RootPath { get; }

    public ZipRootPathInfo(string rootPath)
    {
        Type = RootPathType.Zip;
        RootPath = rootPath;
    }

    private string GetArchivePath(string path) => path.Trim('\\').Replace('\\', '/');

    public string GetFilePathThatEditorCanOpen(string path)
    {
        var archivePath = GetArchivePath(path);
        return CreateZipArchiveDataToTempFile(RootPath, archivePath);
    }

    public byte[] ReadAllBytes(string path)
    {
        var archivePath = GetArchivePath(path);
        return ReadAllBytesForZipFile(RootPath, archivePath);
    }

    public bool Exist(string path)
    {
        var archivePath = GetArchivePath(path);
        return ExistZipFile(RootPath, archivePath);
    }

    private static bool ExistZipFile(string zipFilePath, string archivePath)
    {
        using var zipData = ZipFile.OpenRead(zipFilePath);
        return zipData.GetEntry(archivePath) is not null;
    }

    private static string CreateZipArchiveDataToTempFile(string zipFilePath, string archivePath)
    {
        var tempFilePath = Path.GetTempFileName();
        var bytes = ReadAllBytesForZipFile(zipFilePath, archivePath);
        File.WriteAllBytes(tempFilePath, bytes);
        return tempFilePath;
    }

    private static byte[] ReadAllBytesForZipFile(string zipFilePath, string archivePath)
    {
        using var zipData = ZipFile.OpenRead(zipFilePath);
        var zipEntry = zipData.GetEntry(archivePath);
        if (zipEntry is not null)
        {
            using var stream = zipEntry.Open();
            byte[] byteStream = new byte[stream.Length];
            stream.Read(byteStream, 0, byteStream.Length);
            return byteStream;
        }
        throw new FileNotFoundException("Zipファイルにそのファイルはありません");
    }
}
