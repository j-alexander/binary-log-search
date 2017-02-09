namespace UI.Converters

open System.Windows
open System.Windows.Data

[<ValueConversion(typeof<bool>, typeof<Visibility>)>]
type BooleanToVisibilityConverter() =
    inherit AbstractBooleanConverter<Visibility>(Visibility.Visible, Visibility.Hidden)