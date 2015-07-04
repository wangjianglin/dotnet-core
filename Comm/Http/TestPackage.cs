using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Comm.Http
{
    public class TestPackage:Package
    {
        public TestPackage()
        {
            this.location = "/core/comm/test.action";
            this.RespType = typeof(string);
            //base.Version.Major = 0u;
            //base.Version.Minor = 0u;
        }
        public string data { get; set; }
        public override IDictionary<string, object> GetParams()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("data", data);
            return param;
        }
    }
}
