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
