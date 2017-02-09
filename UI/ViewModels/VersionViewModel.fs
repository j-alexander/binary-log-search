namespace UI.ViewModels

open System.Diagnostics

open UI.Commands
open UI.Models

type VersionViewModel() =

    let model = 
        let model = new VersionModel()
        model.VersionUrl <- @"https://raw.githubusercontent.com/j-alexander/binary-log-search/master/Setup.version"
        model.BrowseUrl <- @"https://github.com/j-alexander/binary-log-search"
        model.Check()
        model

    let browseCommand =
        new RelayCommand(
            (fun _ -> true),
            (fun _ -> model.BrowseUrl |> Process.Start |> ignore))

    member public x.Model = model
    member public x.Browse = browseCommand