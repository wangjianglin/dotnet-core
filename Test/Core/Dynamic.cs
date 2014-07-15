using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AD.Test.Core
{
    //public class TestClass:global::System.Dynamic.DynamicObject
    //{
    //    public string Title { get; set; }
    //}
    [TestClass]
    public class DynamicTest
    {
        [TestMethod]
        public void testDynamic()
        {
            //TestClass obj = new TestClass();
            //var obj2 = new global::System.Dynamic.DynamicObject{ };
            //global::System.Dynamic.DynamicObject obj2 = dynamic { };
            dynamic obj2 = new { Title = "****------*" };
            //obj.Title = "------------";
            showDynamicClass(obj2);
            //Lin.Core.Controls.TaskbarNotifierUtil.Show(obj2);
            //showDynamicClass((dynamic new { Title = "*****" }));
        }

        private static void showDynamicClass(dynamic obj)
        {
            Console.WriteLine("title:" + (obj.Title as string));
        }

        class test1
        {
            public string title;
        }
    }
}
