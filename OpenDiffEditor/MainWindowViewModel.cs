using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            get => _oldDirectoryPath;
            set => SetProperty(ref _oldDirectoryPath, value);
        }

        private string _newDirectoryPath = "";
        public string NewDirectoryPath
        {
            get => _newDirectoryPath;
            set => SetProperty(ref _newDirectoryPath, value);
        }

        public ObservableCollection<DiffFileInfo> FileInfoList { get; } = new ObservableCollection<DiffFileInfo>();

        public IRelayCommand ReloadCommand { get; }

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
        }

    }
}
