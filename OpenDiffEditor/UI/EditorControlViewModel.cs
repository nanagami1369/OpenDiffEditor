using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenDiffEditor.Factory;
using OpenDiffEditor.Model;

namespace OpenDiffEditor.UI;

public class EditorControlViewModel : ObservableObject
{
    private string _oldRootPath = "";
    public string OldRootPath
    {
        get => _oldRootPath.Trim('\"');
        set => SetProperty(ref _oldRootPath, value);
    }

    private string _newRootPath = "";
    public string NewRootPath
    {
        get => _newRootPath.Trim('\"');
        set => SetProperty(ref _newRootPath, value);
    }

    private bool _isFilterAdd = true;
    public bool IsFilterAdd
    {
        get => _isFilterAdd;
        set => SetProperty(ref _isFilterAdd, value);
    }
    private bool _isFilterDelete = true;
    public bool IsFilterDelete
    {
        get => _isFilterDelete;
        set => SetProperty(ref _isFilterDelete, value);
    }
    private bool _isFilterModified = true;
    public bool IsFilterModified
    {
        get => _isFilterModified;
        set => SetProperty(ref _isFilterModified, value);
    }

    public ObservableCollection<DiffFileInfo> FileInfoList { get; } = new ObservableCollection<DiffFileInfo>();

    public IRelayCommand ReloadCommand { get; }
    public IRelayCommand<string> DropOldRootPathCommand { get; }
    public IRelayCommand<string> DropNewRootPathCommand { get; }
    public IRelayCommand<DiffFileInfo> OpenVsCodeCommand { get; }

    public EditorControlViewModel()
    {
        ReloadCommand = new RelayCommand(() =>
        {
            if (!RootPathInfo.IsRootPath(OldRootPath)) { return; }
            if (!RootPathInfo.IsRootPath(NewRootPath)) { return; }
            var diffFileInfo = DiffFileInfoFactory.Factory(OldRootPath, NewRootPath);
            FileInfoList.Clear();
            foreach (var path in diffFileInfo.Where(DiffStatusFilter))
            {
                FileInfoList.Add(path);
            }
        });
        DropOldRootPathCommand = new RelayCommand<string>((path) =>
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                OldRootPath = path;
            }
        });
        DropNewRootPathCommand = new RelayCommand<string>((path) =>
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                NewRootPath = path;
            }
        });
        OpenVsCodeCommand = new RelayCommand<DiffFileInfo>(diffInfo =>
        {
            if (diffInfo is null) { return; }
            diffInfo.OpenDiffEditor((oldFullPath, newFullPath) =>
            {
                var processArgument = diffInfo.Status switch
                {
                    DiffStatus.Add => $"/c code --diff {oldFullPath}",
                    DiffStatus.Delete => $"/c code --diff {oldFullPath}",
                    DiffStatus.Modified => $"/c code --diff {oldFullPath} {newFullPath}",
                    DiffStatus.None => null,
                    _ => throw new NotSupportedException("unknown status")
                };
                // 引数が無ければ終了
                if (string.IsNullOrWhiteSpace(processArgument)) { return; }
                var processStartInfo = new ProcessStartInfo()
                {
                    FileName = "C:\\WINDOWS\\system32\\cmd.exe",
                    Arguments = processArgument,
                    CreateNoWindow = true,
                };
                Process.Start(processStartInfo);
            });
        });
    }

    private bool DiffStatusFilter(DiffFileInfo info)
    {
        return info.Status switch
        {
            DiffStatus.Add => IsFilterAdd,
            DiffStatus.Delete => IsFilterDelete,
            DiffStatus.Modified => IsFilterModified,
            DiffStatus.None => false,
            _ => throw new NotSupportedException("unknown status")
        };
    }
}
