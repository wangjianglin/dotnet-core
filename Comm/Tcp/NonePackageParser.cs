using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    [ProtocolParserType(0)]
    public class NonePackageParser:IProtocolParser
    {
        public Package GetPackage()
        {
            return new ErrorPackage();
        }

        public void Put(params byte[] bs)
        {
        }

        public void Clear()
        {
        }
    }
}
