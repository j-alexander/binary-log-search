namespace UI.Models

open System
open System.Linq
open System.Windows
open System.Collections.ObjectModel
open System.ComponentModel
open System.Diagnostics
open System.Windows.Input

open Algorithm

type QueryModel() =
    inherit DependencyObject()

    let searches = new ObservableCollection<SearchModel>()

    static let logStores = Algorithm.Connections.all
    static let defaultLogStore = Algorithm.Connections.kafka
    static let defaultTarget = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)

    static let defaultHost, defaultChannel, defaultTargetPath, defaultStartAt =
        if Debugger.IsAttached then
            "tcp://guardians-kafka-cluster.qa.jet.com:9092",
            "nova-retailskus-profx2",
            "$.retail_sku_product_data_updated.timestamp",
            0
        else "", "", "$.timestamp", 0

    static let selectedLogStore =
        DependencyProperty.Register("SelectedLogStore", typeof<LogStore>, typeof<QueryModel>, new UIPropertyMetadata(defaultLogStore))
    static let host =
        DependencyProperty.Register("Host", typeof<string>, typeof<QueryModel>, new UIPropertyMetadata(defaultHost))
    static let channel =
        DependencyProperty.Register("Channel", typeof<string>, typeof<QueryModel>, new UIPropertyMetadata(defaultChannel))
    static let startAt =
        DependencyProperty.Register("StartAt", typeof<int>, typeof<QueryModel>, new UIPropertyMetadata(defaultStartAt))
    static let target =
        DependencyProperty.Register("Target", typeof<DateTime>, typeof<QueryModel>, new UIPropertyMetadata(defaultTarget))
    static let targetPath =
        DependencyProperty.Register("TargetPath", typeof<string>, typeof<QueryModel>, new UIPropertyMetadata(defaultTargetPath))
    static let isConnecting =
        DependencyProperty.Register("IsConnecting", typeof<bool>, typeof<QueryModel>, new UIPropertyMetadata(false))
    static let isConnected =
        DependencyProperty.Register("IsConnected", typeof<bool>, typeof<QueryModel>, new UIPropertyMetadata(false))
    static let status =
        DependencyProperty.Register("Status", typeof<string>, typeof<QueryModel>)

    member public x.LogStores
        with get() = logStores

    member public x.SelectedLogStore
        with get() = x.GetValue(selectedLogStore) :?> LogStore
        and set(value:LogStore) = x.SetValue(selectedLogStore, value)

    member public x.Host
        with get() = x.GetValue(host) :?> string
        and set(value:string) = x.SetValue(host, value)

    member public x.Channel
        with get() = x.GetValue(channel) :?> string
        and set(value:string) = x.SetValue(channel, value)

    member public x.StartAt
        with get() = x.GetValue(startAt) :?> int
        and set(value:int) = x.SetValue(startAt, value)

    member public x.Target
        with get() = x.GetValue(target) :?> DateTime
        and set(value:DateTime) = x.SetValue(target, value)

    member public x.TargetPath
        with get() = x.GetValue(targetPath) :?> string
        and set(value:string) = x.SetValue(targetPath, value)

    member public x.IsConnecting
        with get() = x.GetValue(isConnecting) :?> bool
        and set(value:bool) = x.SetValue(isConnecting, value)

    member public x.IsConnected
        with get() = x.GetValue(isConnected) :?> bool
        and set(value:bool) = x.SetValue(isConnected, value)

    member public x.Status
        with get() = x.GetValue(status) :?> string
        and set(value:string) = x.SetValue(status, value)

    member public x.Searches = searches

    member public x.Connect() =
        if not x.IsConnecting then
            x.IsConnecting <- true
            x.IsConnected <- false
            x.Status <- "Connecting..."

            let store = x.SelectedLogStore
            let host = x.Host
            let channel = x.Channel
            let startAt = x.StartAt
            let target = x.Target
            let targetPath = x.TargetPath

            let worker = new BackgroundWorker()
            worker.DoWork.Add(fun e -> 
                let codec = Codec.create(startAt, targetPath)
                let connections = store.connect host channel codec
                let searches =
                    connections
                    |> Array.map(fun x -> new BinaryLogSearch(x))
                e.Result <- searches)
            worker.RunWorkerCompleted.Add(fun e ->
                x.IsConnecting <- false
                if not(isNull e.Error) then
                    x.IsConnected <- false
                    x.Status <- sprintf "Error: %s" e.Error.Message
                elif e.Cancelled then
                    x.IsConnected <- false
                    x.Status <- "Your connection request was cancelled."
                else
                    x.IsConnected <- true
                    x.Status <- "Connected successfully."
                    x.Searches.Clear()
                    for search in e.Result :?> BinaryLogSearch[] do
                        x.Searches.Add(new SearchModel(search, target))
                    CommandManager.InvalidateRequerySuggested())
            worker.RunWorkerAsync()
