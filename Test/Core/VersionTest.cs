using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AD.Test.Core
{
    [TestClass]
    public class VersionTest : BaseTest
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void testVersion()
        {
            //VersionPackage package = new VersionPackage();
            //Request(package, (result, warning) =>
            //{
            //    Console.WriteLine("build:" + ((Lin.Core.Web.Model.Version)result).build);
            //}, error =>
            //{
            //}, 0);
        }

        private int count()
        {
            Console.WriteLine("N:10");
            return 10;
        }
        [TestMethod]
        public void testCount()
        {

            for (int n = 0; n < count(); n++)
            {
                Console.WriteLine("count:" + n);
            }
        }
    }
}
