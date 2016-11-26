using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models;

namespace UI.ViewModels {
    public class MainViewModel {

        public QueryViewModel QueryViewModel { get; set; }

        public SearchViewModel SearchViewModel { get; set; }

        public MainViewModel() {
            QueryViewModel = new QueryViewModel();
            SearchViewModel = new SearchViewModel(QueryViewModel.Model.Searches);
        }
    }
}
