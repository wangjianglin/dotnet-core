using Lin.Util;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lin.Comm.Tcp
{
    public class ProtocolVersionTypeResPackage : Package
    {
        //[CompilerGenerated]
        //private byte[] <AllSustainVersionNumber>k__BackingField;
        //[CompilerGenerated]
        //private byte <SustainVersionNumber>k__BackingField;

        public ProtocolVersionTypeResPackage()
        {
        }

        public int size()
        {
            return (2 + (this.SustainVersionNumber * 3));
            //return 0;
        }

        public override byte[] Write()
        {
            return null;
        }
        public void Write(byte[] bs, int offset = 0)
        {
            ByteUtils.WriteByte(bs, 0xfe, 0);
            ByteUtils.WriteByte(bs, this.SustainVersionNumber, 1);
            ByteUtils.WriteBytes(bs, this.AllSustainVersionNumber, this.AllSustainVersionNumber.Length, 2, 0);
            return;
        }

        public byte[] AllSustainVersionNumber { get; set; }

        public byte SustainVersionNumber { get; set; }

        public override byte Type
        {
            get
            {
                return 0xfe;
            }
        }
    }
}

