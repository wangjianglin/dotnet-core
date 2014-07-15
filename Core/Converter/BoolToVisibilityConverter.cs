using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Lin.Core.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            if (value is Boolean)
            {
                if (((Boolean)value) == true)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            if (value is bool)
            {
                if (((bool)value) == true)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            if(value is long)
            {
                if ((long)value==0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                     
                }
            }

            return value.Equals(0) ? Visibility.Collapsed : Visibility.Visible; 
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility)
            {
                if (((Visibility)value) == Visibility.Visible)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return null;
        }
    }
}
