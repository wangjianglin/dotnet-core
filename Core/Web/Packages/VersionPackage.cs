using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Web.Packages
{
    public class VersionPackage:Package
    {
        public VersionPackage()
        {
            this.location = "/web/action/version_info!get.action";
            this.RespType = typeof(Model.Version);
            this.Version.Major = 0;
            this.Version.Minor = 0;
        }
    }
}
