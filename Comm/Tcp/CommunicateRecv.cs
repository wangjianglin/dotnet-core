using Lin.Util.Assemblys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    internal class CommunicateRecv
    {
        static CommunicateRecv()
        {
            IList<AttributeToken<ProtocolParserType>> tokens = Lin.Util.Assemblys.AssemblyStore.FindAllAttributesForCurrentDomain<ProtocolParserType>(true);
            //Console.WriteLine("count:" + tokens.Count);
            foreach (AttributeToken<ProtocolParserType> token in tokens)
            {
                Communicate.ProtocolParsers[token.Attribute.Type] = token.OwnerType;
            }
        }
        private CommunicateListener listener;
        private Communicate communicate;
        //private ISessionListener sessionListener;
        private Session session;
        public CommunicateRecv(Communicate communicate, Session session, CommunicateListener listener)
        {
            this.communicate = communicate;
            this.listener = listener;
            this.session = session;
            //this.sessionListener = sessionListener;
        }
        private Lin.Util.MapIndexProperty<byte, IProtocolParser> protocolParserInts = new Lin.Util.MapIndexProperty<byte, IProtocolParser>();
        private IProtocolParser getProtocolParser(byte b)
        {
            IProtocolParser parser = protocolParserInts[b];
            if (parser != null)
            {
                return parser;
            }
            parser = (IProtocolParser)Activator.CreateInstance(Communicate.ProtocolParsers[b]);
            protocolParserInts[b] = parser;
            return parser;
        }

        internal void AddRequest(ulong sequeue,Response response){
            sequeues[sequeue] = response;
        }
        //private void CommunicateListener(Session session,Package package,Response pesonse){
        /// <summary>
        /// 无需考虑线程安全问题
        /// </summary>
        private Lin.Util.MapIndexProperty<ulong, Response> sequeues = new Util.MapIndexProperty<ulong, Response>();
        private void CommunicateListener(Package package)
        {
            Response r = sequeues[package.Sequeue];
            if (r != null)
            {
                package.State = PackageState.RESPONSE;
            }
            else
            {
                package.State = PackageState.REQUEST;
            }
                bool isResponse = false;
                Response sr = this.session.Response(package);
            if (this.listener != null)
            {
                //try {
                    listener(this.session, package, p => { isResponse = true; sr(p); });
                //}
                //finally
                //{
                   
                //}
            }
            if (r != null)
            {
                sequeues[package.Sequeue] = null;
                r(package);
            }
            else
            {
                if (!isResponse)
                {
                    sr(new ErrorPackage());
                }
            }
        }

        public unsafe void RecvData()
        {
            const int bufferSize = 2048;
            //byte versionSize;
            //char[] ch = new char[bufferSize];
            byte[] ch = new byte[bufferSize];
            byte dataType = 0;//数据类型

            //bool state = false;//0表示还没有开始读数，true表示正在读取数据,
            //bool isC0 = false;//数据结束标记
            bool isDB = false;//表示前一个数据是否为0xDB
            //bool isFA = false;//0表示前一个数据不是0xFA,true表示前一个数据是0xFA
            bool isFirst = true;//表示当前为此数据表的第一个数据
            //bool isFirstNew = false;
            //int length = 0;//用以记录数据的长度


            int n;

            IProtocolParser parser = null;


            byte[] sequeueBytes = new byte[8];//存储序列号
            int sequeueCount = 0;
            ulong sequeue = 0;

            //数据产生异常
            System.Action initStatue = () =>
            {
                //isC0 = false;
                isDB = false;
                isFirst = true;
                sequeueCount = 0;
                //packageSize = 0;
                if (parser != null)
                {
                    parser.Clear();
                }
                parser = null;
            };
            //PACKAGELISTENER listener;
            //MLog LogFile;
            //LogFile.InitAndWriteLog("communicate listener调用成功");
            while (true)
            {
                //int num = Socket::recv(client,ch,bufferSize,0);
                int num = this.session.Socket.Receive(ch);
                //		cout << "received data numbers=" << num << endl;
                if (num <= 0)
                {//连接已经断开
                    //LogFile.InitAndWriteLog("前台客户端退出 socket close");
                    //cout<<"socket close..."<<endl;
                    break;
                }

                for (n = 0; n < num; n++)
                {
                    if ((byte)ch[n] == 0xC0)
                    {//如果isCo0为true，表示数据结束
                        //isC0 = false;
                        //if (packageSize != 0)
                        //{
                        //if (listener != null && parser != null)
                        if(parser != null)
                        {
                            //Package pack = PackageManager.Parse(packageBody);
                            Package pack = parser.GetPackage();
                            Utils.Read(sequeueBytes, out sequeue);
                            pack.Sequeue = sequeue;
                            initStatue();
                            //listener(null,pack,this.session.Response(pack));
                            CommunicateListener(pack);
                        }
                        continue;
                        //}
                    }

                    if ((byte)ch[n] == 0xDB)
                    {
                        if (isDB)
                        {
                            //异常
                            initStatue();
                            continue;
                        }
                        isDB = true;
                        continue;
                    }
                    if (isDB == true)//如果前一个数据为0xDB，则需要进行数据转义
                    {
                        isDB = false;
                        if ((byte)ch[n] == 0xDC)
                        {
                            ch[n] = 0xC0;
                        }
                        else if ((byte)ch[n] == 0xDD)
                        {
                            ch[n] = 0xDB;
                        }
                        else
                        {
                            //异常，回到初始状态
                            initStatue();
                            continue;
                        }
                    }
                    if (isFirst)
                    {
                        dataType = (byte)ch[n];
                        parser = this.getProtocolParser(dataType);
                        isFirst = false;
                        continue;
                    }
                    if (sequeueCount < 8)
                    {
                        sequeueBytes[sequeueCount++] = ch[n];
                        continue;
                    }
                    if (parser != null)
                    {
                        parser.Put(ch[n]);
                    }
                }
            }
        }
    }
}
