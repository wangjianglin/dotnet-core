using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Lin.Comm.Tcp
{

    public class Communicate
    {

        //private static IDictionary<byte, Type> protocolParsers = new Dictionary<byte, Type>();

        //public static readonly Lin.Util.MapIndexProperty<byte, Type> ProtocolParsers = new Util.MapIndexProperty<byte, Type>();
        
        
        //public bool Connected { get; private set; }
        /// <summary>
        /// 服务端IP
        /// </summary>
        private string ip;
        /// <summary>
        /// 通信监听器
        /// </summary>
        private CommunicateListener listener;
        /// <summary>
        /// 商品号
        /// </summary>
        private int port;
        //private Thread recvThread;
        /// <summary>
        /// 对象锁
        /// </summary>
        private Socket socket;

        private Session clientSession;
        private IList<Session> serverSessions = new List<Session>();
        private ISessionListener sessionListener;

        private bool isServer = false;
        public bool IsServer { get { return isServer; } }
        //public Communicate(CommunicateListener listener, string ip, int port,ISessionListener sessionListener=null)
        public Communicate(CommunicateListener listener, string ip, int port)
        {
            this.listener = listener;
            this.ip = ip;
            this.port = port;
            this.InitClient();
        }

        public Communicate(CommunicateListener listener, int port, ISessionListener sessionListener=null)
        {
            this.listener = listener;
            this.sessionListener = sessionListener;
            this.port = port;
            this.isServer = true;
            this.InitServer();
        }

        public void Close()
        {
            try
            {
                this.socket.Disconnect(false);
            }
            catch (Exception)
            {
            }
            return;
        }

        private void InitServer()
        {
            autoEvent.Reset();
            Thread thread = new Thread(new ThreadStart(InitServerImpl));
            thread.IsBackground = true;
            thread.Start();
            autoEvent.WaitOne();
        }

        private AutoResetEvent autoEvent = new AutoResetEvent(false);

        private void InitServerImpl(){
            try
            {
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.port));
                socket.Listen(0);
                while (true)
                {
                    autoEvent.Set();
                    Socket c = socket.Accept();
                    Session session = new Session(this,c);
                    lock (serverSessions)
                    {
                        serverSessions.Add(session);
                    }
                    if (sessionListener != null)
                    {
                        sessionListener.Create(session);
                    }
                    CommunicateRecv recv = new CommunicateRecv(this, session, listener);
                    session.recv = recv;
                    Thread thread = new Thread(new ThreadStart(()=>{
                        try
                        {
                            recv.RecvData();
                        }
                        finally
                        {
                            lock (serverSessions)
                            {
                                serverSessions.Remove(session);
                                if (sessionListener != null)
                                {
                                    sessionListener.Destory(session);
                                }
                            }
                        }
                    }));
                    thread.IsBackground = true;
                    thread.Start();
                }
            }
            finally
            {
                autoEvent.Set();
            }
        }
        private void InitClient()
        {
            //this.socket = new Socket(2, 1, 0);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                this.socket.Connect(this.ip, this.port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            try
            {
                //this.recvThread.Abort();
            }
            catch (Exception)
            {
            }
            clientSession = new Session(this,socket);
            CommunicateRecv recv = new CommunicateRecv(this, clientSession, listener);
            clientSession.recv = recv;
            Thread thread = new Thread(new ThreadStart(recv.RecvData));
            thread.IsBackground = true;
            thread.Start();
            return;
        }

        public void Reconnection()
        {
            this.Close();
            if (isServer)
            {
                this.InitServer();
            }
            else
            {
                this.InitClient();
            }
            return;
        }

        public PackageResponse Send(Package pack)
        {
            //EnterCriticalSection(&stCommunicateSendPackage);
            if (isServer)
            {
                foreach (Session session in serverSessions)
                {
                    session.Send(pack);
                }
                return null;
            }
            else
            {
                return clientSession.Send(pack);
            }
        }

        /// <summary>
        /// 表示当前的连接状态，true表示连接上，false表示连接断开
        /// </summary>
        public bool Connected
        {
            get
            {
                return this.socket.Connected;
            }
        }
    }
}

