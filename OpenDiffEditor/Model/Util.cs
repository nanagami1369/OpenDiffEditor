using System.IO;
using System.IO.Compression;

namespace OpenDiffEditor.Model;

public static class Util
{
    public static bool IsDirectoryPath(string path) => path is not null && Directory.Exists(path);
    public static bool IsZipPath(string path) => path is not null && File.Exists(path) && Path.GetExtension(path) == ".zip";

    public static bool ExistZipFileZipFile(string zipFilePath, string archivePath)
    {
        using var zipData = ZipFile.OpenRead(zipFilePath);
        return zipData.GetEntry(archivePath) is not null;
    }

    public static string CreateZipArchiveDataToTempFile(string zipFilePath, string archivePath)
    {
        var tempFilePath = Path.GetTempFileName();
        var bytes = ReadAllBytesForZipFile(zipFilePath, archivePath);
        File.WriteAllBytes(tempFilePath, bytes);
        return tempFilePath;
    }

    public static byte[] ReadAllBytesForZipFile(string zipFilePath, string archivePath)
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
