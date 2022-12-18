using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using OpenDiffEditor.Model;

namespace OpenDiffEditor.Factory;

public static class DiffFileInfoFactory
{

    public static IEnumerable<DiffFileInfo> Factory(string oldRootPath, string newRootPath)
    {
        var oldRootPathInfo = RootPathInfo.Create(oldRootPath);
        var newRootPathInfo = RootPathInfo.Create(newRootPath);
        var oldPaths = GetPaths(oldRootPathInfo);
        var newPaths = GetPaths(newRootPathInfo);
        return oldPaths.Union(newPaths).Select(path => DiffFileInfo.Create(oldRootPathInfo, newRootPathInfo, path));
    }

    private static IEnumerable<string> GetPaths(RootPathInfo rootPathInfo)
    {
        switch (rootPathInfo.Type)
        {
            case RootPathType.Directory:
                return Directory.EnumerateFiles(rootPathInfo.RootPath, "*", SearchOption.AllDirectories)
                    .Select(path => path.Replace(rootPathInfo.RootPath, ""))
                    .Select(path => $"\\{path.TrimStart('\\')}");
            case RootPathType.Zip:
                using (var zipData = ZipFile.OpenRead(rootPathInfo.RootPath))
                {
                    return zipData.Entries
                        .Select(data => data.FullName)
                        .Where(path => !path.EndsWith("/"))
                        .Select(path => $"\\{path.Replace('/','\\')}");
                }
            default:
                throw new NotSupportedException("zipとフォルダしか対応してません");
        }
    }
}
