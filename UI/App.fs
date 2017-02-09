namespace UI

open System
open FsXaml


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module App =

    [<STAThread>]
    [<EntryPoint>]
    let main argv =
        let app = UI.Views.App()
        app.Startup.Add(fun _ ->
            let window = new UI.Views.MainWindow()
            app.MainWindow <- window
            app.MainWindow.Show())
        app.Run()