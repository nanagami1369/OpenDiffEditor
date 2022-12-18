using System;
using System.Linq;

namespace OpenDiffEditor.Common.Model
{
    public class DiffFileInfo
    {
        private IRootPathInfo OldRootPathInfo { get; }
        private IRootPathInfo NewRootPathInfo { get; }


        public string Path { get; }
        public DiffStatus Status { get; }
        public string StatusString => Status.ToStringLocal();
        public string StatusIcon => Status.ToIcon();

        private DiffFileInfo(IRootPathInfo oldPathInfo, IRootPathInfo newPathInfo, string path, DiffStatus status)
        {
            OldRootPathInfo = oldPathInfo;
            NewRootPathInfo = newPathInfo;
            Path = path;
            Status = status;
        }

        public static DiffFileInfo Create(IRootPathInfo oldRootPathInfo, IRootPathInfo newRootPathInfo, string path)
        {
            // Diffステータス設定
            var oldFileIsExist = oldRootPathInfo.Exist(path);
            var newFileIsExist = newRootPathInfo.Exist(path);
            DiffStatus status;
            if (oldFileIsExist && newFileIsExist)
            {
                var isModified = !oldRootPathInfo.ReadAllBytes(path).SequenceEqual(newRootPathInfo.ReadAllBytes(path));
                status = isModified ? DiffStatus.Modified : DiffStatus.None;
            }
            else
            {
                status = newFileIsExist ? DiffStatus.Add : DiffStatus.Delete;
            }
            return new DiffFileInfo(oldRootPathInfo, newRootPathInfo, path, status);
        }

        public delegate void ExecOpenEditor(string oldFullPath, string newFullPath);

        public void OpenDiffEditor(ExecOpenEditor execOpenEditorCommand)
        {
            var oldFullPath = Status switch
            {
                DiffStatus.Add => "",
                DiffStatus.Delete => OldRootPathInfo.GetFilePathThatEditorCanOpen(Path),
                DiffStatus.Modified => OldRootPathInfo.GetFilePathThatEditorCanOpen(Path),
                DiffStatus.None => OldRootPathInfo.GetFilePathThatEditorCanOpen(Path),
                _ => throw new NotSupportedException("unknown status")
            };
            var newFullPath = Status switch
            {
                DiffStatus.Add => NewRootPathInfo.GetFilePathThatEditorCanOpen(Path),
                DiffStatus.Delete => "",
                DiffStatus.Modified => NewRootPathInfo.GetFilePathThatEditorCanOpen(Path),
                DiffStatus.None => NewRootPathInfo.GetFilePathThatEditorCanOpen(Path),
                _ => throw new NotSupportedException("unknown status")
            };
            execOpenEditorCommand(oldFullPath, newFullPath);
        }
    }
}