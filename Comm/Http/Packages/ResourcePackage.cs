using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Comm.Http.Packages
{
    /// <summary>
    /// 用于请求服务器上的静态资源
    /// </summary>
    public class ResourcePackage:Package
    {
        public static readonly IHttpRequestHandle RESOURCE = new ResourceHttpRequestHandle();
        public ResourcePackage()
        {
            this.RequestHandle = RESOURCE;
            this.EnableCache = true;
        }
    }
}
