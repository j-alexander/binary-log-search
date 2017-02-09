namespace UI.Converters

open System
open System.Collections.Generic
open System.Globalization
open System.Windows
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Media

[<ValueConversion(typeof<string>, typeof<string>)>]
type SingleLineTextConverter() =

    interface IValueConverter with

        member x.Convert(value:obj, targetType:Type, parameter:obj, culture:CultureInfo) =
            match value with
            | :? string as text ->
                let text = text.Replace("\n"," ")
                let text = text.Replace("\r","")
                if text.Length > 60 then text.Substring(0,60) + "..."
                else text
                :> obj
            | x -> x
        
        member x.ConvertBack(value:obj, targetType:Type, parameter:obj, culture:CultureInfo) =
            raise (new NotImplementedException("Cannot convert a single line of text back to multiline input."))