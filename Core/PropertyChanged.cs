using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Core
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PropertyChanged : Attribute
    {
        public PropertyChanged(string name)
        {
            this.Name = name;

        }

        public string Name { get; private set; }
    }
}
