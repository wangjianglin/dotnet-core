using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{

    [AttributeUsage(AttributeTargets.Property)]
    public class JsonParamsType : Attribute
    {
        public JsonParamsType(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}
