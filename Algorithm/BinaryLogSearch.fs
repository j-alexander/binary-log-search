namespace Algorithm

open System
open System.Threading
open Nata.Core
open Nata.IO

type BinaryLogSearch(connection:LogConnection,
                     startScanAt:int64) =

    let indexOf, readFrom =
        connection.indexOf,
        connection.readFrom

    member x.Name = connection.name;

    member x.Execute (token:CancellationToken,
                      state:SearchState,
                      onStatus:SearchState -> unit) =
    
        let target = state.Target
        let isCancelled _ = token.IsCancellationRequested
        let isNotCancelled = isCancelled >> not
        
        let onStatus(status,position) =
            onStatus({Target=target
                      Status=status
                      Position=Some position})

        let scan (state, position) =
            let max =
                match position.UpperBound - position.Current with
                | x when x >= int64 Int32.MaxValue -> Int32.MaxValue
                | x -> int x
            Position.At position.Current
            |> connection.readFrom
            |> Seq.mapi (fun i (event, index) ->
                if i % 100 = 0 then
                    onStatus(state, { position with Current=index })
                (event, index))
            |> Seq.take max
            |> Seq.takeWhile isNotCancelled
            |> Seq.chooseFst Event.data
        let seek =
            scan 
            >> Seq.tryHead

        let rec search (position:Position) =

            let range = position.UpperBound-position.LowerBound
            if range < startScanAt then
                let position = { position with Current=position.LowerBound }

                scan (Scan, position)
                |> Seq.tryFind (fun (t, at) -> t >= target)
                |> function
                   | Some(t,at) as x ->
                        onStatus(Found(t,at),position)
                        x
                   | None as x ->
                        if isCancelled() then 
                            onStatus(Cancelled, position)
                        else
                            onStatus(NotFound, position)
                        x
            else
                let point = (range/2L)+position.LowerBound
                let position = { position with QueryAt=point; Current=point }

                match seek (Seek, position) with
                | None when isCancelled() ->
                    onStatus(Cancelled,position)
                    None
                | None ->
                    onStatus(NotFound,position)
                    None
                | Some(t, at) when t >= target ->
                    search { position with UpperBound=point }
                | Some(t, at) ->
                    search { position with LowerBound=point }

        //// start search at the start offset
        state.Position
        |> Option.defaultWith(fun () ->
            let lower = indexOf Position.Start
            let upper = indexOf Position.End
            { Position.Current = lower
              Position.QueryAt = lower
              Position.LowerBound = lower
              Position.UpperBound = upper
              Position.Minimum = lower
              Position.Maximum = upper })
        |> search

    