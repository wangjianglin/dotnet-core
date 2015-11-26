using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Lin.Comm.Http
{
    internal class NoneHttpRequestHandle : IHttpRequestHandle
    {
        public byte[] GetParams(HttpWebRequest request, HttpPackage package)
        {
            return null;
        }

        public void Response(HttpPackage package, byte[] bs,Action<Object, IList<Error>> result, Action<Error> fault)
        {
            //result(resp,null);
        }
    }
}
