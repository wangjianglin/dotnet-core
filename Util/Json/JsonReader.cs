using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Util.Json
{
    public class JsonReader
    {
        public object read(string json)
        {
            return JsonObject.Parse(json);
        }
    }
}
