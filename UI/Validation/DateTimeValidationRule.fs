namespace UI.Validation

open System
open System.Globalization
open System.Windows.Controls

type DateTimeValidationRule() =
    inherit ValidationRule()
        
        override x.Validate(value:obj, cultureInfo:CultureInfo) =
            let text = value :?> string
            let pattern = "yyyy-MM-dd HH:mm:ss"
            let culture = CultureInfo.CurrentCulture
            let style = DateTimeStyles.None
            match DateTime.TryParseExact(text, pattern, culture, style) with
            | true, x -> new ValidationResult(true, null)
            | _ -> new ValidationResult(false, "Date Expected: e.g. 2016-01-18T13:45:00.000")