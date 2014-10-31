using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    //[ProtocolParserAttribute]
    public interface IProtocolParser
    {
        Package GetPackage();

        void Put(params byte[] bs);

        void Clear();
    }
}
