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
    public DelegateCommand UsedLicenseCommand { get; }

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
        UsedLicenseCommand = new DelegateCommand(() =>
        {
            var message = @"The MIT License (MIT)

Copyright (c) Prism Library

All rights reserved. Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
            MessageBox.Show(message);
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
