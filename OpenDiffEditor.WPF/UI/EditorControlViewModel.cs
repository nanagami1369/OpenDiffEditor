using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenDiffEditor.Common.Factory;
using OpenDiffEditor.Common.Model;

namespace OpenDiffEditor.WPF.UI;

public class EditorControlViewModel : ObservableObject
{
    private readonly DiffFileInfoFactory _diffFileInfoFactory;

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

        _diffFileInfoFactory = new DiffFileInfoFactory();
        _diffFileInfoFactory.AddRootPathInfoFactory(new DirectoryRootPathInfoFactory());
        _diffFileInfoFactory.AddRootPathInfoFactory(new ZipRootPathInfoFactory());

        ReloadCommand = new RelayCommand(() =>
        {
            if (!_diffFileInfoFactory.IsSupportRootPath(OldRootPath)) { return; }
            if (!_diffFileInfoFactory.IsSupportRootPath(NewRootPath)) { return; }

            var diffFileInfo = _diffFileInfoFactory.Create(OldRootPath, NewRootPath);
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
                var processArgument = $"/c code --diff {oldFullPath} {newFullPath}";
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
