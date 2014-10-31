using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Comm.Http
{
    /// <summary>
    /// 静态资源请求处理器
    /// </summary>
    internal class ResourceHttpRequestHandle : IHttpRequestHandle
    {
        public string GetParams(Packages.Package package)
        {
            throw new NotImplementedException();
        }

        public void Response(Packages.Package package, string resp, Action<object, IList<Error>> result, Action<Error> fault)
        {
            throw new NotImplementedException();
        }
    }
}
