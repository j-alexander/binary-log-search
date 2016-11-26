using System.Windows.Data;

namespace UI.Converters {

    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanConverter : AbstractBooleanConverter<bool> {
        
        public BooleanConverter() : base(true, false) {
        }
    }
}
