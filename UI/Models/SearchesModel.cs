using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UI.Models {

    public class SearchesModel : DependencyObject {
        
        public static readonly DependencyProperty SearchesProperty =
            DependencyProperty.Register("Searches", typeof(ObservableCollection<SearchModel>), typeof(SearchesModel));
        public ObservableCollection<SearchModel> Searches {
            get { return (ObservableCollection<SearchModel>)GetValue(SearchesProperty); }
        }

        public static readonly DependencyProperty SelectedSearchProperty =
            DependencyProperty.Register("SelectedSearch", typeof(SearchModel), typeof(SearchesModel));
        public SearchModel SelectedSearch {
            get { return (SearchModel)GetValue(SelectedSearchProperty); }
            set { SetValue(SelectedSearchProperty, value); }
        }

        public SearchesModel(ObservableCollection<SearchModel> searches) {
            SetValue(SearchesProperty, searches);
        }
    }
}
