namespace UI.Models

open System
open System.ComponentModel
open System.Diagnostics
open System.Net
open System.Reflection
open System.Windows

type VersionModel() =
    inherit DependencyObject()
    
    static let currentVersion =
        let assembly = Assembly.GetExecutingAssembly()
        let fileInfo = FileVersionInfo.GetVersionInfo(assembly.Location)
        fileInfo.FileVersion

    static let upgradeVersion =
        DependencyProperty.Register("UpgradeVersion", typeof<string>, typeof<VersionModel>)
    static let canUpgrade =
        DependencyProperty.Register("CanUpgrade", typeof<bool>, typeof<VersionModel>, new PropertyMetadata(false))
    static let versionUrl =
        DependencyProperty.Register("VersionUrl", typeof<string>, typeof<VersionModel>)
    static let browseUrl =
        DependencyProperty.Register("BrowseUrl", typeof<string>, typeof<VersionModel>)

    member public x.UpgradeVersion
        with get() = x.GetValue(upgradeVersion) :?> string
        and set(value:string) = x.SetValue(upgradeVersion, value)

    member public x.CurrentVersion
        with get() = currentVersion

    member public x.CanUpgrade
        with get() = x.GetValue(canUpgrade) :?> bool
        and set(value:bool) = x.SetValue(canUpgrade, value)

    member public x.VersionUrl
        with get() = x.GetValue(versionUrl) :?> string
        and set(value:string) = x.SetValue(versionUrl, value)

    member public x.BrowseUrl
        with get() = x.GetValue(browseUrl) :?> string
        and set(value:string) = x.SetValue(browseUrl, value)

    member public x.Check() =
        let url = x.VersionUrl
        let worker = new BackgroundWorker()
        worker.DoWork.Add(fun e ->
            let client = new WebClient()
            let result = client.DownloadString(url)
            e.Result <- Version.Parse(result).ToString())
        worker.RunWorkerCompleted.Add(fun e ->
            if isNull e.Error then
                let version = e.Result :?> string
                x.UpgradeVersion <- version
                x.CanUpgrade <- x.CurrentVersion <> version)
        worker.RunWorkerAsync()