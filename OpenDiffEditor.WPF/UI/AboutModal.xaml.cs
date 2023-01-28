using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenDiffEditor.WPF.UI
{
    /// <summary>
    /// AboutModal.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutModal : UserControl
    {
        public AboutModal()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink link)
            {

                var info = new ProcessStartInfo()
                {
                    FileName = link.NavigateUri.ToString(),
                    UseShellExecute = true,
                };
                Process.Start(info);
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
    }
}
