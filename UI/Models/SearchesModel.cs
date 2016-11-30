using System.Collections.ObjectModel;
using System.Windows;

namespace UI.Models {

    public class SearchesModel : DependencyObject {
        
        public static readonly DependencyProperty SearchesProperty =
            DependencyProperty.Register("Searches", typeof(ObservableCollection<SearchModel>), typeof(SearchesModel));
        public ObservableCollection<SearchModel> Searches {
            get { return (ObservableCollection<SearchModel>)GetValue(SearchesProperty); }
        }

        public SearchesModel(ObservableCollection<SearchModel> searches) {
            SetValue(SearchesProperty, searches);
        }
    }
}
