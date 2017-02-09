namespace UI.ViewModels

type MainViewModel() =

    let queryViewModel = new QueryViewModel()
    let searchViewModel = new SearchViewModel(queryViewModel.Model.Searches)

    member public x.QueryViewModel = queryViewModel
    member public x.SearchViewModel = searchViewModel