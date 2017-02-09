namespace UI.ViewModels

open System
open System.Windows.Input

open UI.Commands
open UI.Models

type QueryViewModel() =

    let model = new QueryModel()

    let connectCommand =
        new RelayCommand(
            (fun _ -> not (String.IsNullOrWhiteSpace(model.Host)) &&
                      not (String.IsNullOrWhiteSpace(model.Channel)) &&
                      not (String.IsNullOrWhiteSpace(model.TargetPath))),
            (fun _ -> model.Connect()))

    member public x.Model = model
    member public x.Connect = connectCommand