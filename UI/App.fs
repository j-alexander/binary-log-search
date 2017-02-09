namespace UI

open System
open FsXaml

type App = XAML<"App.xaml">

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module App =

    [<STAThread>]
    [<EntryPoint>]
    let main argv =
        let app = App()
        app.Startup.Add(fun _ ->
            let window = new Views.MainWindow()
            app.MainWindow <- window
            app.MainWindow.Show())
        app.Run()