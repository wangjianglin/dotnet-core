using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    public enum PackageState
    {
        NONE,REQUEST,RESPONSE
    }
    public abstract class Package
    {
        public PackageState State { get;internal set; }
        public Package()
        {
            this.State = PackageState.NONE;
            this.Sequeue = 0;
        }
        public abstract byte Type { get; }

        public ulong Sequeue { get; internal set; }

        public abstract byte[] Write();//byte[] bs, int offset = 0);
    }
}
