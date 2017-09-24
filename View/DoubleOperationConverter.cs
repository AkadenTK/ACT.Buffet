using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Buffet
{
    public class DoubleOperationConverter : MarkupExtension, IValueConverter
    {
        public DoubleOperation Operation
        { get; set; }

        public double Comparator
        { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sValue = value.ToString();
            double dValue;
            if (!double.TryParse(sValue, out dValue)) throw new NotImplementedException();
            switch(Operation)
            {
                case DoubleOperation.EqualTo:
                    return dValue == Comparator;
                case DoubleOperation.GreaterThan:
                    return dValue > Comparator;
                case DoubleOperation.GreaterThanOrEqualTo:
                    return dValue >= Comparator;
                case DoubleOperation.LessThan:
                    return dValue < Comparator;
                case DoubleOperation.LessThanOrEqualTo:
                    return dValue <= Comparator;
            }
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

    public enum DoubleOperation
    {
        EqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo
    }
}
