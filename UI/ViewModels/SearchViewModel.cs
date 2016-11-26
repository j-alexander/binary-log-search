using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UI.Models;

namespace UI.ViewModels {

    public class SearchViewModel {
        
        public SearchesModel Model { get; set; }

        public ICommand Run { get; set; }

        public ICommand RunAll { get; set; }

        public ICommand Stop { get; set; }

        public ICommand StopAll { get; set; }

        public SearchViewModel(ObservableCollection<SearchModel> models) {
            Model = new SearchesModel(models);

            Run = new RelayCommand(
                x => Model?.SelectedSearch?.CanExecute ?? false,
                x => Model.SelectedSearch.Execute());

            Stop = new RelayCommand(
                x => Model?.SelectedSearch?.CanCancel ?? false,
                x => Model.SelectedSearch.Cancel());

            RunAll = new RelayCommand(
                x => Model?.Searches?.Any(search => search.CanExecute) ?? false,
                x => {
                    foreach (var search in Model.Searches.Where(search => search.CanExecute)) {
                        search.Execute();
                    }
                });

            StopAll = new RelayCommand(
                x => Model?.Searches?.Any(search => search.CanCancel) ?? false,
                x => {
                    foreach (var search in Model.Searches.Where(search => search.CanCancel)) {
                        search.Cancel();
                    }
                });
        }
    }
}
