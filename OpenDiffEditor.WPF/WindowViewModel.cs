using OpenDiffEditor.WPF.UI;
using Prism.Commands;
using Prism.Mvvm;

namespace OpenDiffEditor.WPF
{
    public class WindowViewModel : BindableBase
    {
        private bool _isAboutModal = false;
        public bool IsAboutModal
        {
            get => _isAboutModal;
            set => SetProperty(ref _isAboutModal, value);
        }

        public DelegateCommand OpenAboutModalCommand { get; }

        private bool _isSettingModal = false;
        public bool IsSettingModal
        {
            get => _isSettingModal;
            set => SetProperty(ref _isSettingModal, value);
        }

        public DelegateCommand OpenSettingModalCommand { get; }

        public WindowViewModel()
        {
            OpenAboutModalCommand = new DelegateCommand(() =>
            {
                IsAboutModal = true;
                IsSettingModal = false;
                LoadSetting();
            });
            OpenSettingModalCommand = new DelegateCommand(() =>
            {
                IsAboutModal = false;
                IsSettingModal = true;
                LoadSetting();
            });
            SaveSettingModalCommand = new DelegateCommand(() =>
            {
                SaveSetting();
                LoadSetting();
                IsSettingModal = false;
            });
            CloseSettingModalCommand = new DelegateCommand(() =>
            {
                LoadSetting();
                IsSettingModal = false;
            });
        }

        #region SettingModal

        private string _diffCommand = "";
        public string DiffCommand
        {
            get => _diffCommand;
            set => SetProperty(ref _diffCommand, value);
        }

        private string _argument = "";
        public string Argument
        {
            get => _argument;
            set => SetProperty(ref _argument, value);
        }

        public void LoadSetting()
        {
            DiffCommand = Properties.Settings.Default.DiffCommand;
            Argument = Properties.Settings.Default.Argument;
        }

        public void SaveSetting()
        {
            Properties.Settings.Default.DiffCommand = DiffCommand;
            Properties.Settings.Default.Argument = Argument;
            Properties.Settings.Default.Save();
        }

        public DelegateCommand SaveSettingModalCommand { get; }
        public DelegateCommand CloseSettingModalCommand { get; }
        #endregion
    }
}
