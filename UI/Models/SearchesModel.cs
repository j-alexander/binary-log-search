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

        public SearchesModel(ObservableCollection<SearchModel> searches) {
            SetValue(SearchesProperty, searches);
        }
    }
}
