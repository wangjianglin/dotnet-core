using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using Lin.Core.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AD.Test.Core
{
    [TestClass]
    public class ConfigManagerTest
    {
        [TestMethod]
        public void Test()
        {
            TestSync();
        }

        private static void TestProxy()
        {
            ConfigManager.GetConfigManager("Net").PropertyChanged += (object sender, global::System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                Assert.IsTrue(e.PropertyName == "TestConfig");
            };
            AppDomain domain = AppDomain.CreateDomain("TestConfigManger", AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "", false);
            domain.DoCallBack(() =>
            {
                IConfigManager config1 = ConfigManager.GetConfigManager("Net");
                config1["TestConfig"] = "测试配置文件";
                config1.PropertyChanged += (object sender, global::System.ComponentModel.PropertyChangedEventArgs e) =>
                {
                    Assert.IsTrue(config1["TestConfig"] == "测试属性事件改变");
                };
            });

            AppDomain domainGet = AppDomain.CreateDomain("TestGetConfigValue", AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "", false);
            domainGet.DoCallBack(() =>
            {
                IConfigManager config2 = ConfigManager.GetConfigManager("Net");
                Assert.IsTrue(config2["TestConfig"] == "测试配置文件");
                config2["TestConfig"] = "测试属性事件改变";
            });
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
            try
            {
                for (int n = 0; n < 1000; n++)
                {
                    ConfigManager.GetConfigManager("Net")["TestConfig"] = "test1" + n;
                    //Thread.Sleep(100);
                    Assert.IsTrue(ConfigManager.GetConfigManager("Net")["TestConfig"].Equals("test1" + n));
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
                    ConfigManager.GetConfigManager("Net")["TestConfig"] = "test2" + n;
                    Assert.IsTrue(ConfigManager.GetConfigManager("Net")["TestConfig"].Equals("test2" + n));
                }
            }
            finally
            {
                myResetEvent2.Set();
            }
        }
    }
}
