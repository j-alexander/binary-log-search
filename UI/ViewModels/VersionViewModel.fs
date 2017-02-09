namespace UI.ViewModels

open System.Diagnostics
open UI.Models

type VersionViewModel() =

    let model = 
        let model = new VersionModel()
        model.VersionUrl <- @"https://raw.githubusercontent.com/j-alexander/binary-log-search/master/Setup.version"
        model.BrowseUrl <- @"https://github.com/j-alexander/binary-log-search"
        model.Check()
        model

    member public x.Model = model
    member public x.OnBrowseRequest() = model.BrowseUrl |> Process.Start |> ignore