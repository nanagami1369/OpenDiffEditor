using System.IO;

namespace OpenDiffEditor.Common.Model;

public class DirectoryRootPathInfo : IRootPathInfo
{
    public RootPathType Type { get; }
    public string RootPath { get; }

    public DirectoryRootPathInfo(string rootPath)
    {
        Type = RootPathType.Directory;
        RootPath = rootPath;
    }

    private string GetFullPath(string path) => $"{RootPath}\\{path}";

    public string GetFilePathThatEditorCanOpen(string path)
    {
        return GetFullPath(path);
    }

    public byte[] ReadAllBytes(string path)
    {
        var fullPath = GetFullPath(path);
        return File.ReadAllBytes(fullPath);
    }


    public bool Exist(string path)
    {
        var fullPath = GetFullPath(path);
        return File.Exists(fullPath);
    }
}
