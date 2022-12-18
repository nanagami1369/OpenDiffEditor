using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDiffEditor.Model;

public class DiffFileInfo
{
    private RootPathInfo OldRootPathInfo { get; }
    private RootPathInfo NewRootPathInfo { get; }


    public string Path { get; }
    public DiffStatus Status { get; }
    public string StatusString => Status.ToStringLocal();
    public string StatusIcon => Status.ToIcon();

    private DiffFileInfo(RootPathInfo oldPathInfo, RootPathInfo newPathInfo, string path, DiffStatus status)
    {
        OldRootPathInfo = oldPathInfo;
        NewRootPathInfo = newPathInfo;
        Path = path;
        Status = status;
    }

    public static DiffFileInfo Create(RootPathInfo oldRootPathInfo, RootPathInfo newRootPathInfo, string path)
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
        var oldFullPath = OldRootPathInfo.Type switch
        {
            RootPathType.Directory => $"{OldRootPathInfo.RootPath}\\{Path}",
            RootPathType.Zip => Util.CreateZipArchiveDataToTempFile(OldRootPathInfo.RootPath, Path.Trim('\\').Replace('\\', '/')),
            _ => throw new NotSupportedException("zipとフォルダしか対応してません")
        };
        var newFullPath = NewRootPathInfo.Type switch
        {
            RootPathType.Directory => $"{NewRootPathInfo.RootPath}\\{Path}",
            RootPathType.Zip => Util.CreateZipArchiveDataToTempFile(NewRootPathInfo.RootPath, Path.Trim('\\').Replace('\\', '/')),
            _ => throw new NotSupportedException("zipとフォルダしか対応してません")
        };
        execOpenEditorCommand(oldFullPath, newFullPath);
    }
}
