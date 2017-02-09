namespace UI.Converters

open System
open System.Collections.Generic
open System.Globalization
open System.Windows
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Media

[<ValueConversion(typeof<ListViewItem>, typeof<Brush>)>]
type ListViewItemColorConverter() =

    let windowDefault = SystemColors.WindowBrush :> Brush
    let even = ref windowDefault
    let odd = ref windowDefault

    member public x.Even
        with get() = !even
        and set(value) = even := value

    member public x.Odd
        with get() = !odd
        and set(value) = odd := value

    interface IValueConverter with

        member x.Convert(value:obj, targetType:Type, parameter:obj, culture:CultureInfo) =
            match value with
            | :? ListViewItem as item ->
                match ItemsControl.ItemsControlFromItemContainer(item) with
                | :? ListView as container ->
                    let index = container.ItemContainerGenerator.IndexFromContainer(item)
                    if index % 2 = 0 then !even
                    else !odd
                | _ ->
                    windowDefault
            | _ ->
                windowDefault
            :> obj
        
        member x.ConvertBack(value:obj, targetType:Type, parameter:obj, culture:CultureInfo) =
            raise (new NotImplementedException())