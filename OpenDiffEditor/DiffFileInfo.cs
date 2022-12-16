using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDiffEditor
{
    public class DiffFileInfo
    {
        private string OldDirPath { get; }
        private string NewDirPath { get; }


        public string Path { get; }
        public DiffStatus Status { get; }
        public string StatusString => Status.ToStringLocal();


        private static string FullPath(string rootDir, string path) => $"{rootDir}\\{path}";

        public string OldFullPath => FullPath(OldDirPath, Path);
        public string NewFullPath => FullPath(NewDirPath, Path);

        public static DiffFileInfo Create(string oldDirPath, string newDirPath, string path)
        {
            var oldFullPath = FullPath(oldDirPath, path);
            var newFullPath = FullPath(newDirPath, path);
            // Diffステータス設定
            var oldFileIsExist = File.Exists(oldFullPath);
            var newFileIsExist = File.Exists(newFullPath);
            DiffStatus status;
            if (oldFileIsExist && newFileIsExist)
            {
                var isModified = !File.ReadAllBytes(oldFullPath).SequenceEqual(File.ReadAllBytes(newFullPath));
                status = isModified ? DiffStatus.Modified : DiffStatus.None;
            }
            else
            {
                status = newFileIsExist ? DiffStatus.Add : DiffStatus.Delete;
            }

            return new DiffFileInfo(oldDirPath, newDirPath, path, status);
        }

        private DiffFileInfo(string oldDirPath, string newDirPath, string path, DiffStatus status)
        {
            OldDirPath = oldDirPath;
            NewDirPath = newDirPath;
            Path = path;
            Status = status;
        }
    }
}
