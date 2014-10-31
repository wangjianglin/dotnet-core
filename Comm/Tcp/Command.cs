using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Command : Attribute
    {
        public Command(int command)
        {
            this.Commaand = command;
        }
        public int Commaand { get; private set; }
    }
}
