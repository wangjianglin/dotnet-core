using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "Lin.Core.Model")]

#endregion

namespace Lin.Core
{
    public class NameValueModel:Model
    {
        private string name;
        public string Name { set { this.name = value; this.OnPropertyChanged("Name"); } get { return this.name; } }

        private object value;
        public object Value { set { this.value = value; this.OnPropertyChanged("Value"); } get { return this.value; } }
    }
}
