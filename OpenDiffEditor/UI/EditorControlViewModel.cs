using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpenDiffEditor.UI;

public class EditorControlViewModel : ObservableObject
{
    private string _oldDirectoryPath = "";
    public string OldDirectoryPath
    {
        get => _oldDirectoryPath.Trim('\"');
        set => SetProperty(ref _oldDirectoryPath, value);
    }

    private string _newDirectoryPath = "";
    public string NewDirectoryPath
    {
        get => _newDirectoryPath.Trim('\"');
        set => SetProperty(ref _newDirectoryPath, value);
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
    public IRelayCommand<string> DropOldDirPathCommand { get; }
    public IRelayCommand<string> DropNewDirPathCommand { get; }
    public IRelayCommand<DiffFileInfo> OpenVsCodeCommand { get; }

    public EditorControlViewModel()
    {
        ReloadCommand = new RelayCommand(() =>
        {
            if (OldDirectoryPath is null || !Directory.Exists(OldDirectoryPath)) { return; }
            if (NewDirectoryPath is null || !Directory.Exists(NewDirectoryPath)) { return; }

            var oldPaths = Directory.EnumerateFiles(OldDirectoryPath, "*", SearchOption.AllDirectories)
                .Select(path => path.Replace(OldDirectoryPath, ""));
            var newPaths = Directory.EnumerateFiles(NewDirectoryPath, "*", SearchOption.AllDirectories)
                .Select(path => path.Replace(NewDirectoryPath, ""));

            var diffFileInfo = oldPaths.Union(newPaths).Select(path => DiffFileInfo.Create(OldDirectoryPath, NewDirectoryPath, path));
            FileInfoList.Clear();
            foreach (var path in diffFileInfo.Where(DiffStatusFilter))
            {
                FileInfoList.Add(path);
            }
        });
        DropOldDirPathCommand = new RelayCommand<string>((path) =>
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                OldDirectoryPath = path;
            }
        });
        DropNewDirPathCommand = new RelayCommand<string>((path) =>
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                NewDirectoryPath = path;
            }
        });
        OpenVsCodeCommand = new RelayCommand<DiffFileInfo>(diffInfo =>
        {
            if (diffInfo is null) { return; }
            var processArgument = diffInfo.Status switch
            {
                DiffStatus.Add => $"--diff {diffInfo.NewFullPath}",
                DiffStatus.Delete => $"--diff {diffInfo.OldFullPath}",
                DiffStatus.Modified => $"--diff {diffInfo.OldFullPath} {diffInfo.NewFullPath}",
                // それ以外は、null
                _ => null
            };
            // 引数が無ければ終了
            if (string.IsNullOrWhiteSpace(processArgument)) { return; }

            var processStartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = "code",
                Arguments = processArgument
            };
            Process.Start(processStartInfo);
        });
    }

    private bool DiffStatusFilter(DiffFileInfo info)
    {
        return info.Status switch
        {
            DiffStatus.Add => IsFilterAdd,
            DiffStatus.Delete => IsFilterDelete,
            DiffStatus.Modified => IsFilterModified,
            _ => false
        };
    }
}
