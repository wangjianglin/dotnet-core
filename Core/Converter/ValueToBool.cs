using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Lin.Core.Converter
{
    /// <summary>
    /// 当等于指定的值时，返回true，否则返回false
    /// </summary>
    public class ValueToBool : IValueConverter
    {
        ///// <summary>
        ///// 当绑定的值与些相同时，返回true，否则返回false
        ///// </summary>
        public object Value { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            this.Value = value;
            if ((parameter == null && value == null)
                || (parameter != null && parameter.Equals(value))
            || (parameter != null && value != null && parameter.ToString() == value.ToString())
                )
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (true.Equals(value))
            {
                return parameter;
            }
            return this.Value;  
        }
    }
}
