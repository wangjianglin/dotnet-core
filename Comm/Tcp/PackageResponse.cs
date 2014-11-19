using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    //class PackageResponse
    public class PackageResponse
        {
            private AutoResetEvent set;
            internal PackageResponse(AutoResetEvent set)
            {
                this.set = set;
            }

            private volatile Package pack;
            internal void Response(Package pack)
            {
                this.pack = pack;
                set.Set();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="timeout">超时，以毫秒为单位，默认120秒</param>
            /// <returns></returns>
            public Package WaitForEnd(int timeout = 120000)
            {
                set.WaitOne();
                return this.pack;
            }
        }
}
