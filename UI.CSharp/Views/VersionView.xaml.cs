using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
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
