using System;
using System.Windows.Input;
using UI.Models;

namespace UI.ViewModels {

    public class QueryViewModel {

        public QueryModel Model { get; set; }

        public ICommand Connect { get; set; }

        public QueryViewModel() {
            Model = new QueryModel();

            Connect = new RelayCommand(
                x => !String.IsNullOrWhiteSpace(Model.Host) &&
                     !String.IsNullOrWhiteSpace(Model.Channel) &&
                     !String.IsNullOrWhiteSpace(Model.TargetPath) &&
                     Model.Target != null,
                x => Model.Connect());
        }
    }
}
