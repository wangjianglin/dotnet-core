using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;


#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "Lin.Core.Converter")]

#endregion

namespace Lin.Core.Converter
{
    /// <summary>
    /// 
    /// </summary>
    public class BoolNotConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertImpl(value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ConvertImpl(value);
        }

        private object ConvertImpl(object value)
        {
            return !BoolValue(value);
        }

        private bool BoolValue(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is Boolean)
            {
                return (Boolean)value;
            }

            if (value is bool)
            {
                return (bool)value;
            }

            return value.Equals(0) ? false : true;
        }
    }
}
