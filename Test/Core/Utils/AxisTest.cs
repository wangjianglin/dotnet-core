using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lin.Core.Utils;

namespace AD.Test.Core.Utils
{
    [TestClass]
    public class AxisTest
    {
        [TestMethod]
        public void TestStep()
        {
            Console.WriteLine("0.1:" + Axis.Step(1000, 0.1));
            Console.WriteLine("104570:" + Axis.Step(1000, 54570));
        }
    }
}
