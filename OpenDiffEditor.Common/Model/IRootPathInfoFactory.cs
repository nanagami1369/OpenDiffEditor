using System.Collections.Generic;

namespace OpenDiffEditor.Common.Model
{
    public interface IRootPathInfoFactory
    {
        bool IsRootPath(string rootPath);

        IEnumerable<string> ReadPaths(string rootPath);

        IRootPathInfo Create(string rootPath);
    }
}
