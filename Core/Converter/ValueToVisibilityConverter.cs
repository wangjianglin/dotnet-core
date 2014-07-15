using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Lin.Core.Converter
{
    public class ValueToVisibilityConverter : IValueConverter
    {
        ///// <summary>
        ///// 当绑定的值与参数相同时，返回true，否则返回false
        ///// </summary>
        //public object Value { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((parameter == null && value == null)
                || (parameter != null && parameter.Equals(value))
            || (parameter != null && value != null && parameter.ToString() == value.ToString())
                )
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Visibility.Visible.Equals(value))
            {
                return parameter;
            }
            throw new NotImplementedException();
        }
    }
}
