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


        public WindowViewModel()
        {
            OpenAboutModalCommand = new DelegateCommand(() => IsAboutModal = true);
        }
    }
}
