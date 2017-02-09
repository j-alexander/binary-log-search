namespace UI.Converters

open System.Windows.Data

[<ValueConversion(typeof<bool>, typeof<bool>)>]
type BooleanConverter() =
    inherit AbstractBooleanConverter<bool>(true,false)