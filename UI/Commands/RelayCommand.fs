namespace UI.Commands

open System
open System.Windows.Input

type RelayCommand(canExecute, execute) =
    interface ICommand with
        
        [<CLIEvent>]
        member x.CanExecuteChanged = CommandManager.RequerySuggested
        member x.CanExecute(parameter) = canExecute parameter
        member x.Execute(parameter) = execute parameter