using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace OpenDiffEditor
{
    public class MainWindowViewModel : ObservableObject
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

        public ObservableCollection<DiffFileInfo> FileInfoList { get; } = new ObservableCollection<DiffFileInfo>();

        public IRelayCommand ReloadCommand { get; }
        public IRelayCommand<string> DropOldDirPathCommand { get; }
        public IRelayCommand<string> DropNewDirPathCommand { get; }
        public IRelayCommand<DiffFileInfo> OpenVsCodeCommand { get; }

        public MainWindowViewModel()
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
                foreach (var path in diffFileInfo)
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
                var processInfo = diffInfo.Status switch
                {
                    DiffStatus.Add => new ProcessStartInfo()
                    {
                        UseShellExecute = true,
                        FileName = "code",
                        Arguments = $"--diff {diffInfo.NewFullPath}"
                    },
                    DiffStatus.Delete => new ProcessStartInfo()
                    {
                        UseShellExecute = true,
                        FileName = "code",
                        Arguments = $"--diff {diffInfo.OldFullPath}"
                    },
                    DiffStatus.Modified => new ProcessStartInfo()
                    {
                        UseShellExecute = true,
                        FileName = "code",
                        Arguments = $"--diff {diffInfo.OldFullPath} {diffInfo.NewFullPath}"
                    },
                    // それ以外は、null
                    _ => null
                };
                if (processInfo is not null)
                {
                    Process.Start(processInfo);
                }
            });
        }

    }
}
