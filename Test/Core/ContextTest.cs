using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AD.Test.Core
{
    [TestClass]
    public class ContextTest
    {
        [TestMethod]
        public void TestContextSyn()
        {
            TestSync();
        }

        private static void TestProxy()
        {
            //Lin.Core.ViewModel.Context.Global.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            //{
            //    Console.WriteLine("Come in");
            //};

            //AppDomain domain = AppDomain.CreateDomain("TestContext", AppDomain.CurrentDomain.Evidence,
            //    AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "", false);
            //domain.DoCallBack(() =>
            //{
            //    IContext context = Context.Global;
            //    context["context"] = "测试全局应用程序";
            //    context.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            //    {
            //        Assert.IsTrue(context["context"].ToString() == "测试全局应用程序属性改变事件");
            //    };
            //});

            //AppDomain domainGet = AppDomain.CreateDomain("TestGetContext", AppDomain.CurrentDomain.Evidence,
            //   AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "", false);
            //domainGet.DoCallBack(() =>
            //{
            //    Assert.IsTrue(Context.Global["context"].ToString() == "测试全局应用程序");
            //    Context.Global["context"] = "测试全局应用程序属性改变事件";
            //});
        }

        static AutoResetEvent myResetEvent1 = new AutoResetEvent(false);
        static AutoResetEvent myResetEvent2 = new AutoResetEvent(false);
        static void TestSync()
        {
            Thread testThread = new Thread(new ThreadStart(test1));
            Thread testThread2 = new Thread(new ThreadStart(test2));
            testThread.Start();
            testThread2.Start();

            myResetEvent1.WaitOne();
            myResetEvent2.WaitOne();
            Console.WriteLine("end.");
        }

        static void test1()
        {
            //try
            //{
            //    for (int n = 0; n < 1000; n++)
            //    {
            //        Context.Global["context"] = "test1" + n;
            //         Thread.Sleep(100);
            //        Assert.IsTrue(Context.Global["context"].Equals("test1" + n));
            //    }
            //}
            //finally
            //{
            //    myResetEvent1.Set();
            //}
        }

        static void test2()
        {
            //try
            //{
            //    for (int n = 0; n < 1000; n++)
            //    {
            //        Context.Global["context"] = "test2" + n;
            //        Assert.IsTrue(Context.Global["context"].Equals("test2" + n));
            //    }
            //}
            //finally
            //{
            //    myResetEvent2.Set();
            //}
        }
    }
}
