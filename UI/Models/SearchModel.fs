namespace UI.Models

open System
open System.ComponentModel
open System.Threading
open System.Windows
open System.Windows.Input

open Algorithm

type SearchModel(search : BinaryLogSearch, target : DateTime) =
    inherit DependencyObject()

    let cancel : Ref<unit->unit> = ref id
    let state : Ref<SearchState> = ref { SearchState.empty with Target=target }
    
    static let targetResult =
        DependencyProperty.Register("TargetResult", typeof<Nullable<DateTime>>, typeof<SearchModel>)
    static let lowerBound =
        DependencyProperty.Register("LowerBound", typeof<int64>, typeof<SearchModel>)
    static let upperBound =
        DependencyProperty.Register("UpperBound", typeof<int64>, typeof<SearchModel>)
    static let minimum =
        DependencyProperty.Register("Minimum", typeof<int64>, typeof<SearchModel>)
    static let maximum =
        DependencyProperty.Register("Maximum", typeof<int64>, typeof<SearchModel>)
    static let range =
        DependencyProperty.Register("Range", typeof<int64>, typeof<SearchModel>)
    static let total =
        DependencyProperty.Register("Total", typeof<int64>, typeof<SearchModel>)
    static let queryPosition =
        DependencyProperty.Register("QueryPosition", typeof<int64>, typeof<SearchModel>)
    static let currentPosition =
        DependencyProperty.Register("CurrentPosition", typeof<int64>, typeof<SearchModel>)
    static let status =
        DependencyProperty.Register("Status", typeof<string>, typeof<SearchModel>)
    static let isExecuting =
        DependencyProperty.Register("IsExecuting", typeof<bool>, typeof<SearchModel>)
    static let isCancelling =
        DependencyProperty.Register("IsCancelling", typeof<bool>, typeof<SearchModel>)

    member public x.LogName = search.Name

    member public x.Target = target

    member public x.TargetResult
        with get() = x.GetValue(targetResult) :?> Nullable<DateTime>
        and set(value:Nullable<DateTime>) = x.SetValue(targetResult, value)

    member public x.LowerBound
        with get() = x.GetValue(lowerBound) :?> int64
        and set(value:int64) = x.SetValue(lowerBound, value)

    member public x.UpperBound
        with get() = x.GetValue(upperBound) :?> int64
        and set(value:int64) = x.SetValue(upperBound, value)

    member public x.Minimum
        with get() = x.GetValue(minimum) :?> int64
        and set(value:int64) = x.SetValue(minimum, value)

    member public x.Maximum
        with get() = x.GetValue(maximum) :?> int64
        and set(value:int64) = x.SetValue(maximum, value)

    member public x.Range
        with get() = x.GetValue(range) :?> int64
        and set(value:int64) = x.SetValue(range, value)

    member public x.Total
        with get() = x.GetValue(total) :?> int64
        and set(value:int64) = x.SetValue(total, value)

    member public x.QueryPosition
        with get() = x.GetValue(queryPosition) :?> int64
        and set(value:int64) = x.SetValue(queryPosition, value)

    member public x.CurrentPosition
        with get() = x.GetValue(currentPosition) :?> int64
        and set(value:int64) = x.SetValue(currentPosition, value)

    member public x.Status
        with get() = x.GetValue(status) :?> string
        and set(value:string) = x.SetValue(status, value)

    member public x.IsExecuting
        with get() = x.GetValue(isExecuting) :?> bool
        and set(value:bool) = x.SetValue(isExecuting, value)

    member public x.IsCancelling
        with get() = x.GetValue(isCancelling) :?> bool
        and set(value:bool) = x.SetValue(isCancelling, value)

    member public x.IsFound =
        x.Status = "Found"

    member public x.CanExecute =
        not x.IsExecuting && not x.IsFound

    member public x.CanCancel =
        x.IsExecuting && not x.IsCancelling

    member public x.Cancel() = (!cancel)()

    member public x.Execute() =
        if x.CanExecute then
            x.IsExecuting <- true

            let tokenSource = new CancellationTokenSource()

            cancel := fun () ->
                if x.CanCancel then
                    x.IsCancelling <- true
                    tokenSource.Cancel(false)

            let worker = new BackgroundWorker()
            worker.WorkerReportsProgress <- true
            worker.WorkerSupportsCancellation <- true
            worker.DoWork.Add(fun e ->
                search.Execute(tokenSource.Token, !state, fun state ->
                    worker.ReportProgress(0, state))
                |> ignore)
            worker.ProgressChanged.Add(fun e ->
                state := e.UserState :?> SearchState
                let state = !state

                match state.Position with
                | None -> ()
                | Some position ->
                    if x.Maximum <> position.Maximum then
                        x.Maximum <- position.Maximum

                    if x.Minimum <> position.Minimum then
                        x.Minimum <- position.Minimum

                    if x.LowerBound <> position.LowerBound then
                        x.LowerBound <- position.LowerBound

                    if x.UpperBound <> position.UpperBound then
                        x.UpperBound <- position.UpperBound

                    if x.QueryPosition <> position.QueryAt then
                        x.QueryPosition <- position.QueryAt

                    if x.CurrentPosition <> position.Current then
                        x.CurrentPosition <- position.Current

                    let range = position.UpperBound - position.LowerBound
                    if x.Range <> range then
                        x.Range <- range

                    let total = position.Maximum - position.Minimum
                    if x.Total <> total then
                        x.Total <- total

                let status = 
                    match state.Status with
                    | Idle ->
                        "Idle"
                    | Scan ->
                        "Scan"
                    | Seek ->
                        "Seek"
                    | Cancelled ->
                        "Cancelled"
                    | NotFound ->
                        "NotFound"
                    | Found (at, index) ->
                        x.TargetResult <- Nullable(at)
                        x.UpperBound <- index
                        x.LowerBound <- index
                        x.QueryPosition <- index
                        x.CurrentPosition <- index
                        x.Range <- 0L
                        "Found"
                if x.Status <> status then
                    x.Status <- status)
            worker.RunWorkerCompleted.Add(fun e ->
                x.IsCancelling <- false
                x.IsExecuting <- false
                if not(isNull e.Error) then
                    x.Status <-
                        if e.Error.Message.Contains "Invalid JSON" then "Invalid JSON"
                        else sprintf "Error: %s" e.Error.Message)
            worker.RunWorkerAsync()
