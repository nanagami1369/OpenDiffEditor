using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using OpenDiffEditor.Common.Factory;
using OpenDiffEditor.Common.Model;
using Prism.Commands;
using Prism.Mvvm;

namespace OpenDiffEditor.WPF.UI;

public class EditorControlViewModel : BindableBase
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

    public DelegateCommand ReloadCommand { get; }
    public DelegateCommand<string> DropOldRootPathCommand { get; }
    public DelegateCommand<string> DropNewRootPathCommand { get; }
    public DelegateCommand<DiffFileInfo> OpenVsCodeCommand { get; }

    public EditorControlViewModel()
    {

        _diffFileInfoFactory = new DiffFileInfoFactory();
        _diffFileInfoFactory.AddRootPathInfoFactory(new DirectoryRootPathInfoFactory());
        _diffFileInfoFactory.AddRootPathInfoFactory(new ZipRootPathInfoFactory());

        ReloadCommand = new DelegateCommand(() =>
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
        DropOldRootPathCommand = new DelegateCommand<string>((path) =>
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                OldRootPath = path;
            }
        });
        DropNewRootPathCommand = new DelegateCommand<string>((path) =>
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                NewRootPath = path;
            }
        });
        OpenVsCodeCommand = new DelegateCommand<DiffFileInfo>(diffInfo =>
        {
            if (diffInfo is null) { return; }
            diffInfo.OpenDiffEditor((oldFullPath, newFullPath) =>
            {
                var diffCommand =
                    Environment.ExpandEnvironmentVariables(Properties.Settings.Default.DiffCommand);
                var Argument =
                    Environment.ExpandEnvironmentVariables(Properties.Settings.Default.Argument)
                    .Replace("{old}", oldFullPath)
                    .Replace("{new}", newFullPath);

                var processStartInfo = new ProcessStartInfo()
                {
                    FileName = diffCommand,
                    Arguments = Argument,
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
