namespace UI.Converters

open System
open System.Collections.Generic
open System.Globalization
open System.Windows
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Media

open Algorithm

[<ValueConversion(typeof<Target option>, typeof<string>)>]
type TargetOptionConverter() =

    interface IValueConverter with

        member x.Convert(value:obj, targetType:Type, parameter:obj, culture:CultureInfo) =
            match value with
            | :? Option<Target> as target ->
                match target with
                | Some (Target.Timestamp x) -> x.ToString("yyyy-MM-dd HH:mm:ss")
                | Some (Target.Text x) -> x
                | Some (Target.Number x) -> x.ToString()
                | None -> null
                :> obj
            | x -> x
        
        member x.ConvertBack(value:obj, targetType:Type, parameter:obj, culture:CultureInfo) =
            match value with
            | :? string as value ->
                match DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", culture, DateTimeStyles.None) with
                | true, x -> Target.Timestamp x
                | _ ->
                    match Decimal.TryParse(value) with
                    | true, x -> Target.Number x
                    | _ -> Target.Text value
                |> Some
                :> obj
            | x ->
                None
                :> obj