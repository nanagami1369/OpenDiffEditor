using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenDiffEditor.Model;

public class DirectoryRootPathInfoFactory : IRootPathInfoFactory
{
    public IRootPathInfo Create(string rootPath)
    {
        return new DirectoryRootPathInfo(rootPath);
    }

    public bool IsRootPath(string rootPath)
    {
        return rootPath is not null && Directory.Exists(rootPath);
    }

    public IEnumerable<string> ReadPaths(string rootPath)
    {
        return Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories)
            .Select(path => path.Replace(rootPath, ""))
            .Select(path => $"\\{path.TrimStart('\\')}");
    }
}
