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
        string GetParams(HttpWebRequest request,Package package);

        void Response(Package package, string resp, Action<Object, IList<Error>> result, Action<Error> fault);
    }
}
