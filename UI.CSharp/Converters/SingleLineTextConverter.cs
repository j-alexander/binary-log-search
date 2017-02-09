using System;
using System.Globalization;
using System.Windows.Data;

namespace UI.Converters {

    [ValueConversion(typeof(string), typeof(string))]
    public class SingleLineTextConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null)
                return value;

            var input = (string)value;
            input = input.Replace("\n", " ").Replace("\r", "");
            if (input.Length > 60)
                input = input.Substring(0, 60) + "...";
            return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException("Cannot convert a single line of text back to multiline input.");
        }
    }
}
