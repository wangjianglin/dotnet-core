using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "Lin.Core.Model")]

#endregion

namespace Lin.Core.Model
{
    public class NameValue
    {
        public string Name { set; get; }

        public object Value { get; set; }
    }
}
