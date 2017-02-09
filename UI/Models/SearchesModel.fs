namespace UI.Models

open System.Collections.ObjectModel
open System.Windows

type SearchesModel(searches:ObservableCollection<SearchModel>) =

    member public x.Searches = searches