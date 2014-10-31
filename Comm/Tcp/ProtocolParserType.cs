using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProtocolParserType : Attribute
    {

        public ProtocolParserType(byte type)
        {
            this.Type = type;
        }
        public byte Type { get; private set; }
    }
}
