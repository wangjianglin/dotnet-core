using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JsonPath : Attribute
    {
        public JsonPath(string path)
        {
            if (!path.EndsWith("/"))
            {
                this.Path = path + "/";
            }
        }

        public string Path { get; private set; }
    }
}
