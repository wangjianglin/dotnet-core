using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Web.Json
{
    public class JsonReader
    {
        public object read(string json)
        {
            return JsonObject.Parse(json);
        }
    }
}
