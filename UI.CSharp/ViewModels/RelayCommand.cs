using System;
using System.Windows.Input;

namespace UI.ViewModels {

    public class RelayCommand : ICommand {

        private Predicate<object> CanExecute { get; set; }
        private Action<object> Execute { get; set; }

        public RelayCommand(Predicate<object> canExecute, Action<object> execute) {
            CanExecute = canExecute;
            Execute = execute;
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        event EventHandler ICommand.CanExecuteChanged {
            add { CanExecuteChanged += value; }
            remove { CanExecuteChanged -= value; }
        }

        bool ICommand.CanExecute(object parameter) { return CanExecute(parameter); }

        void ICommand.Execute(object parameter) { Execute(parameter); }
    }
}