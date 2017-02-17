namespace UI.ViewModels

open System
open System.Collections
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Text
open System.Windows
open System.Windows.Input

open UI.Commands
open UI.Models

type SearchViewModel(models:ObservableCollection<SearchModel>) =

    let model = new SearchesModel(models)

    let relaySelected (select:obj->seq<SearchModel>, canExecute, execute) =
        new RelayCommand(
            select >> canExecute,
            select >> execute)
        :> ICommand

    let run select =
        relaySelected(select,
            (Seq.exists (fun x -> x.CanExecute)),
            (Seq.filter (fun x -> x.CanExecute) >> Seq.iter (fun x -> x.Execute())))

    let stop (select:obj->seq<SearchModel>) =
        relaySelected(select,
            (Seq.exists (fun x -> x.CanCancel)),
            (Seq.filter (fun x -> x.CanCancel) >> Seq.iter (fun x -> x.Cancel())))
        
    let selected : obj -> _ =
        function :? IList as xs -> xs |> Seq.cast<SearchModel> | _ -> Seq.empty
    let all =
        function _ -> model.Searches :> seq<SearchModel>

    let runCommand = run selected
    let runAllCommand = run all
    let stopCommand = stop selected
    let stopAllCommand = stop all

    let copyCommand =
        relaySelected(
            selected,
            Seq.isEmpty >> not,
            fun selection ->
                let builder = new StringBuilder()

                let format =
                    function
                    | x when (obj.ReferenceEquals(null, x)) -> ""
                    | x ->
                        x.ToString()
                         .Replace("\n","")
                         .Replace("\r","")
                         .Replace("\t","")

                let column value =
                    builder.AppendFormat("{0}\t", format value)
                    |> ignore
                let row _ =
                    builder.AppendLine()
                    |> ignore
                
                column("Name")
                column("Result")
                column("Total")
                column("Range")
                column("From")
                column("To")
                column("Query")
                column("Current")
                column("Status")
                row()
                for search in selection do
                    column(search.LogName)
                    column(search.TargetResult)
                    column(search.Total)
                    column(search.Range)
                    column(search.LowerBound)
                    column(search.UpperBound)
                    column(search.QueryPosition)
                    column(search.CurrentPosition)
                    column(search.Status)
                    row()

                Clipboard.SetText(builder.ToString()))
            


    member public x.Model = model
    member public x.Run = runCommand
    member public x.RunAll = runAllCommand
    member public x.Stop = stopCommand
    member public x.StopAll = stopAllCommand
    member public x.Copy = copyCommand
        


    