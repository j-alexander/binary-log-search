using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace UI.Converters {

    public abstract class AbstractBooleanConverter<T> : IValueConverter {

        public T True { get; set; }
        public T False { get; set; }

        public AbstractBooleanConverter(T whenTrue, T whenFalse) {
            True = whenTrue;
            False = whenFalse;
        }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is bool && ((bool)value) ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }
}
