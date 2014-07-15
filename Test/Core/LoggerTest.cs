using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lin.Core.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AD.Test.Core
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void TestLogger()
        {
            //ThreadStart ts = new ThreadStart(() =>
            //{
            //    Logger.Logs["testLoggerProxy"].Info("测试线程同步问题");
            //});
            //ThreadStart ts1 = new ThreadStart(() =>
            //{
            //    Logger.Logs["testLoggerProxy"].Info("测试线程同步问题1");
            //});
            //Thread t = new Thread(ts);
            //Thread t1 = new Thread(ts1);
            //t.Start();
            //t1.Start();
            //Console.ReadLine();
            TestSync();
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

        object obj = new object();
        static void test1()
        {
            try
            {
                for (int n = 0; n < 1000; n++)
                {
                    Thread.Sleep(100);
                    Logger.Logs["testLoggerProxy"].Info("测试线程同步问题1" + n);
                }
            }
            finally
            {
                myResetEvent1.Set();
            }
        }

        static void test2()
        {
            try
            {
                for (int n = 0; n < 1000; n++)
                {
                    Logger.Logs["testLoggerProxy"].Info("测试线程同步问题" + n);
                }
            }
            finally
            {
                myResetEvent2.Set();
            }
        }
    }
}
