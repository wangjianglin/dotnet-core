using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Collections;


namespace Lin.Core.Converter
{
    public class NullToBoolConverter:IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {//parameter 为true时，表示为null "" count() == 0时，是返回true 
            //parameter 为false时，表示为null时，是返回false
            if (true.Equals(parameter) || (parameter != null && "true".Equals(parameter.ToString().ToLower())))
            {
                //，value为null,返回true,
                if (value == null || "".Equals(value))
                {
                    return true;
                }
                IEnumerable list = value as IEnumerable;
                if (list != null)
                {
                    IEnumerator er = list.GetEnumerator();
                    int size = 0;
                    while (er.MoveNext()) { size++; break; }
                    if (size == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if (value == null || "".Equals(value))
                {
                    return false;
                }
                IEnumerable list = value as IEnumerable;
                if (list != null)
                {
                    IEnumerator er = list.GetEnumerator();
                    int size = 0;
                    while (er.MoveNext()) { size++; break; }
                    if (size == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
