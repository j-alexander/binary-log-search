using System;
using System.Collections.Generic;
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
using UI.ViewModels;

namespace UI.Views {

    public partial class VersionView : UserControl {

        public VersionViewModel ViewModel { get; set; }

        public VersionView() {
            InitializeComponent();

            DataContext = ViewModel = new VersionViewModel();
        }

        private void Upgrade_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            ViewModel.OnBrowseRequest();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            ViewModel.OnBrowseRequest();
        }
    }
}
