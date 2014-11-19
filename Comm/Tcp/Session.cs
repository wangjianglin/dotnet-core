using Lin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    public class Session
    {
        public Lin.Util.MapIndexProperty<string, Object> Attributes { get; private set; }


        private object SendPackageLock = new object();
        private ulong sequeue = 1;
        public Socket Socket { get; private set; }
        private Communicate communicate;
        internal CommunicateRecv recv;
        internal Session(Communicate communicate, Socket socket)
        {
            if (communicate.IsServer)
            {
                sequeue = 2;
            }
            this.Socket = socket;
            Attributes = new Util.MapIndexProperty<string, object>();
            this.communicate = communicate;
        }

        public String SessionId { get; private set; }


        private object sequeueLock = new object();
        public PackageResponse Send(Package pack)
        {
            lock (sequeueLock)
            {
                if (this.sequeue == 0)
                {
                    this.sequeue += 2;
                }
                pack.Sequeue = this.sequeue;
                this.sequeue += 2;
            }

            AutoResetEvent set = new AutoResetEvent(false);
            PackageResponse r = new PackageResponse(set);
            recv.AddRequest(pack.Sequeue,r.Response);
            this.SendImpl(pack,true);
            return r;
        }

        internal Response Response(Package pack)
        {
            return (p) =>
            {
                p.Sequeue = pack.Sequeue;
                this.SendImpl(p,false);
            };
        }

        private void SendImpl(Package pack,bool isRequest)
        {
            //int size = pack.size();
            //byte[] bs = new byte[size];
            byte[] bs = pack.Write();
            byte[] tmpBs = new byte[2 * bs.Length + 3 + 18];	//14
            int pos = 0;
            //将包中的数据写入数组
            tmpBs[0] = 0xC0;
            tmpBs[1] = pack.Type;
            if (isRequest)
            {
                tmpBs[2] = 0;
            }
            else
            {
                tmpBs[2] = 1;
            }

            ByteUtils.WriteLong(tmpBs, (long)pack.Sequeue, 3);
            pos = 11;

            for (int n = 0; n < bs.Length; n++)
            {				//解析FF、FA
                if ((byte)bs[n] == 0xC0)
                {
                    tmpBs[pos] = 0xDB;
                    pos++;
                    tmpBs[pos] = 0xDC;
                    pos++;
                }
                else if ((byte)bs[n] == 0xDB)
                {
                    tmpBs[pos] = 0xDB;
                    pos++;
                    tmpBs[pos] = 0xDD;
                    pos++;
                }
                else
                {
                    tmpBs[pos] = bs[n];
                    pos++;
                }
            }
            tmpBs[pos] = 0xC0;

            lock (this.SendPackageLock)
            {
                this.Socket.Send(tmpBs, pos + 1, SocketFlags.None);
            }
        }

        
    }
}
