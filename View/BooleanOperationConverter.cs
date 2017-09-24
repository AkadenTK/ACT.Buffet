using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Buffet
{
    public class BooleanOperationConverter : IMultiValueConverter
    {
        public BooleanOperation Operation { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool output = (Operation == BooleanOperation.And) ? true : false;
            foreach (var value in values)
            {
                bool? bValue = value as bool?;
                if (bValue.HasValue)
                {
                    switch (Operation)
                    {
                        case BooleanOperation.And:
                            output &= bValue.Value;
                            break;
                        case BooleanOperation.Or:
                            output |= bValue.Value;
                            break;
                        case BooleanOperation.Xor:
                            if (bValue == true && output == true) return false;
                            output |= bValue.Value;
                            break;
                    }
                }
                else
                    throw new NotImplementedException();
            }
            return output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum BooleanOperation
    {
        And,
        Or,
        Xor
    }
}
