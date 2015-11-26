using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Lin.Comm.Http
{
    public interface IHttpRequestHandle
    {
        /// <summary>
        /// 根据数据包构造请求参数
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        byte[] GetParams(HttpWebRequest request, HttpPackage package);

        void Response(HttpPackage package, byte[] bs, Action<Object, IList<Error>> result, Action<Error> fault);
    }
}
