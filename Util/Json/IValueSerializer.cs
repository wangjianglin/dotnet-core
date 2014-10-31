using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Lin.Util.Json
{
    public interface IValueSerializer
    {
        object Serialize(object value, System.Type targetType, object parameter, CultureInfo culture);

        object Deserialize(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture);
    }
}
