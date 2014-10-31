using System;
using System.Runtime.CompilerServices;
    
namespace Lin.Comm.Tcp
{
    [Command(-0x1)]
    public class DetectPackageResp : CommandPackage
    {

        public DetectPackageResp()
            //: base(-0x1)
        {
            return;
        }


        public override void Parser(byte[] bs)
        {
            //throw new NotImplementedException();
        }
    }
}

