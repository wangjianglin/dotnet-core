using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Lin.Comm.Http
{
    internal class NoneHttpRequestHandle : IHttpRequestHandle
    {
        public string GetParams(HttpWebRequest request, Package package)
        {
            return null;
        }

        public void Response(Package package, string resp,Action<Object, IList<Error>> result, Action<Error> fault)
        {
            result(resp,null);
        }
    }
}
