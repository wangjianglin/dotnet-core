using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Web.Http
{
    internal class NoneHttpRequestHandle : IHttpRequestHandle
    {
        public string GetParams(Packages.Package package)
        {
            return null;
        }

        public void Response(Packages.Package package, string resp,Action<Object, IList<Error>> result, Action<Error> fault)
        {
            result(resp,null);
        }
    }
}
