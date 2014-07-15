using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections;

namespace Lin.Core.Converter
{
    public class MutilConverter : IValueConverter
    {
        public MutilConverter()
        {
            Converters = new List<IValueConverter>();
        }
        public System.Collections.Generic.List<IValueConverter> Converters { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = value;
            if (Converters != null)
            {
                for (int n = 0; n < Converters.Count; n++)
                {
                    result = Converters.ElementAt(n).Convert(result, targetType, parameter, culture);
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = value;
            if (Converters != null)
            {
                for (int n = Converters.Count-1; n >=0; n--)
                {
                    result = Converters.ElementAt(n).ConvertBack(result, targetType, parameter, culture);
                }
            }
            return result;
        }
    }
}
