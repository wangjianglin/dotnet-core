using Lin.Comm.Http.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Comm.Http
{
    public class FileDeletePackage:Package
    {
        public FileDeletePackage()
        {
            this.Version.Minor = 0;
            this.Version.Major = 0;
            this.RespType = null;
            this.location = "/cloud/action/file!delete.action";
        }

        public string key { get; set; }

        public override IDictionary<string, object> GetParams()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("key", key);
            return param;
        }
    }
}
