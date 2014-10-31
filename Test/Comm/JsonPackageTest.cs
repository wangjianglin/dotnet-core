using Lin.Comm.Tcp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Test.Comm
{
    [TestClass]
    public class JsonPackageTest
    {
        [TestMethod]
        public void TestJsonPackageParser()
        {
            Lin.Comm.Tcp.JsonTestPackage test = new Lin.Comm.Tcp.JsonTestPackage();
            //Lin.Comm.Tcp.JsonTestPackage result = new Lin.Comm.Tcp.JsonTestPackage();
            //result.Parser(test.Write());
            //Console.WriteLine("result:" + result.Params);
            JsonProtocolParser parser = new JsonProtocolParser();
            parser.Put(test.Write());
            JsonTestPackage result = parser.GetPackage() as JsonTestPackage;
        }
    }
}
