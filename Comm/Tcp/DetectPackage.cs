using System;
using System.Runtime.CompilerServices;

namespace Lin.Comm.Tcp
{
    //[PackageAttribute(DetectPackage.Parser)]
    [Command(0x1)]
    public class DetectPackage : CommandPackage
    {
        //static DetectPackage()
        //{
        //    PackageManager.RegisterPackage(1, (byte[] bs)=>{
        //        return new DetectPackage();
        //    });
        //    return;
        //}

        public override void Parser(byte[] bs)
        {
            //return new DetectPackage();
        }

        public DetectPackage()//:base(1)
        {
        }
    }
}

