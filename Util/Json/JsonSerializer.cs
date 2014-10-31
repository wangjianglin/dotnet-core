using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Util.Json
{
    public class JsonSerializer : Attribute
    {
        public JsonSerializer(Type serializer)
        {
            this.Serializer = serializer;
        }

        public Type Serializer { get; private set; }
        public object Parameter { get; set; }
    }
}
