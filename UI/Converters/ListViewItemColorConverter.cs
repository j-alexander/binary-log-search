using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace UI.Converters {

    [ValueConversion(typeof(ListViewItem), typeof(Brush))]
    public class ListViewItemColorConverter : IValueConverter {

        public Brush Even { get; set; }
        public Brush Odd { get; set; }

        public ListViewItemColorConverter() {
            Even = SystemColors.WindowBrush;
            Odd = SystemColors.WindowBrush;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var item = value as ListViewItem;
            if (item == null) {
                return SystemColors.WindowBrush;
            } else {
                var container = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
                var index = container.ItemContainerGenerator.IndexFromContainer(item);
                return index % 2 == 0 ? Even : Odd;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
