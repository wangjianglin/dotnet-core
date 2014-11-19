using Lin.Comm.Tcp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AD.Test.Comm
{
    [TestClass]
    public class CommTest
    {
        class SessionListener : Lin.Comm.Tcp.ISessionListener
        {

            public void Create(Lin.Comm.Tcp.Session session)
            {
                Console.WriteLine("session create.");
            }

            public void Destory(Lin.Comm.Tcp.Session session)
            {
                Console.WriteLine("session destory.");
            }
        }

        [TestMethod]
        public void TestSequeue()
        {
            ulong seq = ulong.MaxValue - 4;
            for (int n = 0; n < 5; n++)
            {
                Console.WriteLine("sequeue:" + seq);
                seq += 2;
            }

            Console.WriteLine("------------------------------------");
            seq = ulong.MaxValue - 5;
            for (int n = 0; n < 5; n++)
            {
                Console.WriteLine("sequeue:" + seq);
                seq += 2;
            }
        }
        [TestMethod]
        public void TestComm()
        {
            Lin.Comm.Tcp.Communicate server = new Lin.Comm.Tcp.Communicate((session,pack,response) =>
            {
                CommandPackage p = pack as CommandPackage;
                if (p != null)
                {
                    Console.WriteLine("pack:" + ((CommandPackage)pack).Command);
                }
                JsonPackage json = pack as JsonPackage;
                if (json != null)
                {
                    Console.WriteLine("json:" + json.Path);
                }
                //response(pack);
            }, 7890,new SessionListener());

            Lin.Comm.Tcp.Communicate client = new Lin.Comm.Tcp.Communicate((session, pack,response) =>
            {

            }, "127.0.0.1", 7890);

            Lin.Comm.Tcp.DetectPackage detect = new Lin.Comm.Tcp.DetectPackage();
            Package r = client.Send(detect).WaitForEnd();
            Console.WriteLine("----------------------------------"+r.Sequeue+r);

            Lin.Comm.Tcp.JsonTestPackage jsonPack = new JsonTestPackage();
            jsonPack.Data = "test.";
            r = client.Send(jsonPack).WaitForEnd();
            Console.WriteLine("----------------------------------" + r.Sequeue+r);

            //Thread.Sleep(1000);
            client.Close();

            server.Close();
            //Thread.Sleep(1000);
            Console.WriteLine("end .");
            //Console.ReadKey();
        }


        [TestMethod]
        public void TestServerComm()
        {
            Lin.Comm.Tcp.Communicate server = new Lin.Comm.Tcp.Communicate((session, pack, response) =>
            {
                CommandPackage p = pack as CommandPackage;
                if (p != null)
                {
                    Console.WriteLine("pack:" + ((CommandPackage)pack).Command);
                }
                JsonPackage json = pack as JsonPackage;
                if (json != null)
                {
                    Console.WriteLine("json:" + json.Path);
                }
                //response(pack);
            }, 7890, new SessionListener());

            //Lin.Comm.Tcp.Communicate client = new Lin.Comm.Tcp.Communicate((session, pack, response) =>
            //{

            //}, "127.0.0.1", 7890);

            //Lin.Comm.Tcp.DetectPackage detect = new Lin.Comm.Tcp.DetectPackage();
            //Package r = client.Send(detect).WaitForEnd();
            //Console.WriteLine("----------------------------------" + r.Sequeue + r);

            //Lin.Comm.Tcp.JsonTestPackage jsonPack = new JsonTestPackage();
            //jsonPack.Data = "test.";
            //r = client.Send(jsonPack).WaitForEnd();
            //Console.WriteLine("----------------------------------" + r.Sequeue + r);

            ////Thread.Sleep(1000);
            //client.Close();

            //server.Close();
            Thread.Sleep(20000);
            //Console.WriteLine("end .");
            ////Console.ReadKey();
        }
    }
}
