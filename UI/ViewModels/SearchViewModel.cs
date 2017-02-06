using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using UI.Models;

namespace UI.ViewModels {

    public class SearchViewModel {
        
        public SearchesModel Model { get; set; }

        public ICommand Run { get; set; }

        public ICommand RunAll { get; set; }

        public ICommand Stop { get; set; }

        public ICommand StopAll { get; set; }

        public ICommand Copy { get; set; }

        public SearchViewModel(ObservableCollection<SearchModel> models) {
            Model = new SearchesModel(models);

            Func<Func<object,IEnumerable<SearchModel>>,
                 Func<IEnumerable<SearchModel>,bool>,
                 Action<IEnumerable<SearchModel>>,
                 RelayCommand> relaySelected = (select, canExecute, execute) =>
                    new RelayCommand(
                        x => canExecute(select(x)),
                        x => execute(select(x)));

            Func<Func<object, IEnumerable<SearchModel>>, RelayCommand> run = (select) =>
                 relaySelected(select,
                     x => x?.Any(search => search.CanExecute) ?? false,
                     x => {
                         foreach (var search in x.Where(search => search.CanExecute))
                             search.Execute();
                     });

            Func<Func<object,IEnumerable<SearchModel>>, RelayCommand> stop = (select) =>
                 relaySelected(select,
                     x => x?.Any(search => search.CanCancel) ?? false,
                     x => {
                         foreach (var search in x.Where(search => search.CanCancel))
                             search.Cancel();
                     });
            
            Run = run(x => (x as IList)?.Cast<SearchModel>());
            RunAll = run(x => Model?.Searches);

            Stop = stop(x => (x as IList)?.Cast<SearchModel>());
            StopAll = stop(x => Model?.Searches);

            Copy = relaySelected(
                x => (x as IList)?.Cast<SearchModel>(),
                x => x.Count() > 0,
                x => {
                    var builder = new StringBuilder();

                    Action<object> column = (value) =>
                        builder.AppendFormat("{0}\t", 
                            value?.ToString()
                                 ?.Replace("\n","")
                                 ?.Replace("\r", "")
                                 ?.Replace("\t","") ?? "");
                    Action row = () =>
                        builder.AppendLine();

                    column("Name");
                    column("Result");
                    column("Range");
                    column("From");
                    column("To");
                    column("Query");
                    column("Current");
                    column("Status");
                    row();
                    foreach (var search in x) {
                        column(search.LogName);
                        column(search.TargetResult);
                        column(search.Range);
                        column(search.LowerBound);
                        column(search.UpperBound);
                        column(search.QueryPosition);
                        column(search.CurrentPosition);
                        column(search.Status);
                        row();
                    }
                    Clipboard.SetText(builder.ToString());
                });
        }
    }
}
