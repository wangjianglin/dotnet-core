using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Web.Http
{
    public interface IHttpRequestHandle
    {
        /// <summary>
        /// 根据数据包构造请求参数
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        string GetParams(Packages.Package package);

        void Response(Packages.Package package, string resp, Action<Object, IList<Error>> result, Action<Error> fault);
    }
}
