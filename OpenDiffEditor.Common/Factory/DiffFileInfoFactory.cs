using System.Collections.Generic;
using System;
using System.Linq;
using OpenDiffEditor.Common.Model;

namespace OpenDiffEditor.Common.Factory;

public class DiffFileInfoFactory
{
    private List<IRootPathInfoFactory> RootPathAccessSystemFactories = new();

    public void AddRootPathInfoFactory(IRootPathInfoFactory factory)
    {
        RootPathAccessSystemFactories.Add(factory);
    }

    public bool IsSupportRootPath(string rootPath)
    {
        return RootPathAccessSystemFactories.Exists(f => f.IsRootPath(rootPath));
    }

    public IEnumerable<DiffFileInfo> Create(string oldRootPath, string newRootPath)
    {
        var oldRootPathFactory = RootPathAccessSystemFactories.Find(f => f.IsRootPath(oldRootPath));
        if (oldRootPathFactory is null) { throw new NotSupportedException("対応していない形式です"); }
        var newRootPathFactory = RootPathAccessSystemFactories.Find(f => f.IsRootPath(newRootPath));
        if (newRootPathFactory is null) { throw new NotSupportedException("対応していない形式です"); }

        var oldPaths = oldRootPathFactory.ReadPaths(oldRootPath);
        var newPaths = newRootPathFactory.ReadPaths(newRootPath);

        var oldRootPathInfo = oldRootPathFactory.Create(oldRootPath);
        var newRootPathInfo = newRootPathFactory.Create(newRootPath);
        return oldPaths.Union(newPaths).Select(path => DiffFileInfo.Create(oldRootPathInfo, newRootPathInfo, path));
    }
}
