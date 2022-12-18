using System;
using System.IO;
using System.IO.Compression;

namespace OpenDiffEditor.Model
{
    public class RootPathInfo
    {
        public RootPathType Type { get; }
        public string RootPath { get; }

        private RootPathInfo(RootPathType fileType, string rootPath)
        {
            Type = fileType;
            RootPath = rootPath;
        }

        public static RootPathInfo Create(string rootPath)
        {
            if (Util.IsDirectoryPath(rootPath))
            {
                return new RootPathInfo(RootPathType.Directory, rootPath);
            }
            if (Util.IsZipPath(rootPath))
            {
                return new RootPathInfo(RootPathType.Zip, rootPath);
            }
            throw new NotSupportedException("zipとフォルダしか対応してません");
        }

        public bool Exist(string path)
        {
            switch (Type)
            {
                case RootPathType.Directory:
                    var fullPath = $"{RootPath}\\{path}";
                    return File.Exists(fullPath);
                case RootPathType.Zip:
                    var archivePath = path.Trim('\\').Replace('\\', '/');
                    return Util.ExistZipFileZipFile(RootPath, archivePath);
                default:
                    throw new NotSupportedException("zipとフォルダしか対応してません");
            }
        }

        public byte[] ReadAllBytes(string path)
        {
            switch (Type)
            {
                case RootPathType.Directory:
                    var fullPath = $"{RootPath}\\{path}";
                    return File.ReadAllBytes(fullPath);
                case RootPathType.Zip:
                    var archivePath = path.Trim('\\').Replace('\\', '/');
                    return Util.ReadAllBytesForZipFile(RootPath, archivePath);
                default:
                    throw new NotSupportedException("zipとフォルダしか対応してません");
            }
        }

        public static bool IsRootPath(string path)
        {
            if (Util.IsDirectoryPath(path)) { return true; }
            if (Util.IsZipPath(path)) { return true; }
            return false;
        }

    }
}
