using System.Windows;
using System.Windows.Data;

namespace UI.Converters {

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : AbstractBooleanConverter<Visibility> {

        public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Hidden) {
        }
    }
}
