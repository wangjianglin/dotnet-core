using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace Lin.Core.Converter
{
    public class BoolNotToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            bool v = BoolValue(value);

            if (v == true)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
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

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility)
            {
                if (((Visibility)value) == Visibility.Visible)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
