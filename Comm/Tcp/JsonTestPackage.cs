using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    [JsonPath("/json/test")]
    public class JsonTestPackage:JsonPackage
    {
        public JsonTestPackage()
        {
            Data = "test.";
        }

        [JsonParamsType(typeof(string))]
        protected override object Params { get { return Data; } set { this.Data = value as string; } }

        public string Data { get; set; }
    }
}
