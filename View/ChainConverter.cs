using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Buffet
{
    public class ChainConverter : List<IValueConverter>, IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach(var converter in this)
            {
                value = converter.Convert(value, targetType, parameter, culture);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach(var converter in this.Reverse<IValueConverter>())
            {
                value = converter.ConvertBack(value, targetType, parameter, culture);
            }
            return value;
        }
    }
    public class MultiValueChainConverter : List<IValueConverter>, IMultiValueConverter
    {
        public IMultiValueConverter InitialConverter
        { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object value = InitialConverter.Convert(values, targetType, parameter, culture);
            foreach(var converter in this)
            {
                value = converter.Convert(value, targetType, parameter, culture);
            }
            return value;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
