using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    /// <summary>
    /// 请求错误响应包
    /// </summary>
    public class ErrorPackage:Package
    {
        public override byte Type { get { return 0; } }
        //private byte _type;

        public override byte[] Write()
        {
            return new byte[0];
        }
    }
}
