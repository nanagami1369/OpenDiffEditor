using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using OpenDiffEditor.Model;

namespace OpenDiffEditor.Factory
{
    public class ZipRootPathInfoFactory : IRootPathInfoFactory
    {
        public IRootPathInfo Create(string rootPath)
        {
            return new ZipRootPathInfo(rootPath);
        }

        public bool IsRootPath(string rootPath)
        {
            return rootPath is not null && File.Exists(rootPath) && Path.GetExtension(rootPath) == ".zip";
        }

        public IEnumerable<string> ReadPaths(string rootPath)
        {
            using var zipData = ZipFile.OpenRead(rootPath);
            return zipData.Entries
                .Select(data => data.FullName)
                .Where(path => !path.EndsWith("/"))
                .Select(path => $"\\{path.Replace('/', '\\')}");
        }
    }
}
