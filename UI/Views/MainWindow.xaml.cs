using System.Windows;
using UI.ViewModels;

namespace UI.Views {

    public partial class MainWindow : Window {

        public MainViewModel ViewModel { get; set; }

        public MainWindow() {
            InitializeComponent();

            ViewModel = new MainViewModel();
            QueryView.DataContext = ViewModel.QueryViewModel;
            SearchView.DataContext = ViewModel.SearchViewModel;
        }
    }
}
