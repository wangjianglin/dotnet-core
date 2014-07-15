using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Lin.Core.Converter
{
    public class CheckedValue :IValueConverter
    {
        private object Value { get; set; }
         
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this.Value = value; 
            try
            {
                int pos = int.Parse(parameter.ToString());  
                if (value is int || value is uint)
                {
                    int? v = value as int?;
                    int? v2 = v & (1 << pos);
                    if (v2 != 0)
                    {
                        return true;
                    }
                }
                else if (value is long || value is ulong)
                {
                    long? v = value as long?;
                    long? v2 = v & (1 << pos);
                    if (v2 != 0)
                    {
                        return true;
                    }
                }
                else if (value is Int16 || value is UInt16)
                {
                    //short? v = value as short?;
                    //short? v3 = v & (1 << pos);
                    //if (v3 != 0)
                    //{
                    //    return true;
                    //}
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            int pos = int.Parse(parameter.ToString());

            if (this.Value is int)
            { 
                int? val = this.Value as int?;
                if (b == true)
                {
                    this.Value = val | (1 << pos);
                }
                else
                {
                    this.Value = val & (~(1 << pos));
                }
            }
            if(this.Value is long)
            { 
                long? val = this.Value as long?;
                if (b == true)
                {
                    this.Value = val | (1 << pos);
                }
                else
                {
                    this.Value = val & (~(1 << pos));
                } 
            }
            if(this.Value is Int16)
            {
                short? val = this.Value as short?;
                if (b == true)
                {
                    this.Value = val | (1 << pos);
                }
                else
                {
                    this.Value = val & (~(1 << pos));
                } 
            }
            return this.Value;
        }
    }
}
