using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lin.Comm.Tcp
{
    public class ProtocolVersionTypePackage : Package
    {

        public ProtocolVersionTypePackage()
        {
        }

        public int size()
        {
            return (2 + (this.SustainVersionNumber * 3));
        }

        public override byte[] Write()
        {
            return null;
        }
        public void Write(byte[] bs, int offset = 0)
        {
        //    Utils.Write(bs, 0xff, 0);
        //    Utils.Write(bs, this.SustainVersionNumber, 1);
        //    if (this.AllSustainVersionNumber == null)
        //    {
        //        goto Label_0037;
        //    }
        //    Utils.Write(bs, this.AllSustainVersionNumber, (int) this.AllSustainVersionNumber.Length, 2, 0);
        //Label_0037:
        //    return;
        }

        public byte[] AllSustainVersionNumber{get;set;}

        public byte SustainVersionNumber { get; set; }

        public override byte Type
        {
            get
            {
                return 0xff;
            }
        }
    }
}

