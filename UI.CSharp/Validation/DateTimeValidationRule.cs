using System;
using System.Globalization;
using System.Windows.Controls;

namespace UI.Validation {
    public class DateTimeValidationRule : ValidationRule {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            DateTime output;
            if (DateTime.TryParseExact(value as string, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out output)) {
                return new ValidationResult(true, null);
            } else {
                return new ValidationResult(false, "Date Expected: e.g. 2016-01-18T13:45:00.000");
            }
        }
    }
}
