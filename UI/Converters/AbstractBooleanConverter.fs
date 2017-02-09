namespace UI.Converters

open System
open System.Collections.Generic
open System.Globalization
open System.Windows.Data

[<AbstractClass>]
type AbstractBooleanConverter<'T>(trueValue:'T, falseValue:'T) =

    let trueValue = ref trueValue
    let falseValue = ref falseValue

    member public x.True
        with get() = !trueValue
        and set(value) = trueValue := value

    member public x.False
        with get() = !falseValue
        and set(value) = falseValue := value

    interface IValueConverter with

        member x.Convert(value:obj, targetType:Type, parameter:obj, culture:CultureInfo) =
            match value with
            | :? bool as isTrue -> if isTrue then !trueValue else !falseValue
            | _ -> !falseValue
            :> obj
        
        member x.ConvertBack(value:obj, targetType:Type, parameter:obj, culture:CultureInfo) =
            match value with
            | :? 'T as tValue -> EqualityComparer<'T>.Default.Equals(tValue, !trueValue)
            | _ -> false
            :> obj