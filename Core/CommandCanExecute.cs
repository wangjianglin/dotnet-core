using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandCanExecute : Attribute
    {
        public CommandCanExecute(string name)
        {
            this.Name = name;

        }

        public string Name { get; private set; }
    }
}
