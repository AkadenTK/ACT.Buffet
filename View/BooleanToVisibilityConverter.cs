using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Buffet
{
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public bool HideOnFalse = false;
        public bool Invert = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            if (boolValue.HasValue)
            {
                if (Invert) boolValue = !boolValue.Value;
                if (boolValue.Value) return Visibility.Visible;
                else return (HideOnFalse) ? Visibility.Hidden : Visibility.Collapsed;
            }
            else
                throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
