using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lin.Core.Web.Http;
using System.IO;
using System.Threading;

namespace AD.Test.Core
{
    [TestClass]
    public class FileTest
    {
        [TestMethod]
        public void TestCopy()
        {
            FileInfo file = new FileInfo(@"d:\X16-42552VS2010UltimTrial1.txt");
            AutoResetEvent are = new AutoResetEvent(false);
            string key = null;
            HttpCommunicate.Upload(file, (result, warning) =>
            {
                Console.WriteLine("result:" + result);
                key = result + "";
                are.Set();
            }, error =>
            {
                Console.WriteLine("error:" + error);
                are.Set();
            });
            are.WaitOne();
            are.Reset();
            FileCopyPackage package = new FileCopyPackage();
            package.key = key;
            HttpCommunicate.Request(package, (result, warning) =>
            {
                Console.WriteLine("file:" + result);
                are.Set();
            }, error =>
            {
                Console.WriteLine("error:" + error);
                are.Set();
            });
            are.WaitOne();
        }

        [TestMethod]
        public void TestDelete()
        {
            FileInfo file = new FileInfo(@"d:\X16-42552VS2010UltimTrial1.txt");
            AutoResetEvent are = new AutoResetEvent(false);
            string key = null;
            HttpCommunicate.Upload(file, (result, warning) =>
            {
                Console.WriteLine("result:" + result);
                key = result + "";
                are.Set();
            }, error =>
            {
                Console.WriteLine("error:" + error);
                are.Set();
            });
            are.WaitOne();
            are.Reset();
            FileDeletePackage package = new FileDeletePackage();
            package.key = key;
            HttpCommunicate.Request(package, (result, warning) =>
            {
                Console.WriteLine("file:" + result);
                are.Set();
            }, error =>
            {
                Console.WriteLine("error:" + error);
                are.Set();
            });
            are.WaitOne();
        }

        [TestMethod]
        public void TestDownload()
        {
            //FileInfo file = new FileInfo(@"d:\X16-42552VS2010UltimTrial1.txt");
            FileInfo file = new FileInfo(@"d:\LibSrc.zip");
            string key = null;
           HttpCommunicateResult r= HttpCommunicate.Upload(file, (result, warning) =>
            {
                Console.WriteLine("result:" + result);
                key = result + "";
            }, error =>
            {
                Console.WriteLine("error:" + error);
            },(a, b) =>
            {
                Console.WriteLine(a + "-" + b);

            });


           r.WaitForEnd();

            HttpCommunicate.Download(key, (result, warning) =>
            {
                Console.WriteLine("file:" + result.FileInfo.FullName);
            }, error =>
            {
                Console.WriteLine("error:" + error);
            },(a, b) =>
            {
                Console.WriteLine(a + "-" + b);
            }).WaitForEnd();

        }

        [TestMethod]
        public void TestUrlDownload()
        {
            Lin.Core.Web.Http.HttpCommunicate.Download(new Uri("http://192.168.1.18:8080/download/ecm/dotnet/update.zip"),
                (file, warnning) =>
                {
                    Console.WriteLine("ok!");
                }, err =>
                {
                    Console.WriteLine("fault!");
                }, (a, b) =>
            {
                Console.WriteLine(a + "-" + b);
            }).WaitForEnd();
        }

        [TestMethod] 
        public void TestHttpUpload()
        {
            //FileInfo file = new FileInfo(@"d:\X16-42552VS2010UltimTrial1.txt");
            FileInfo file = new FileInfo(@"d:\LibSrc.zip");
            
            AutoResetEvent are = new AutoResetEvent(false);

            HttpCommunicateResult hresult = HttpCommunicate.Upload(file, (result, warning) =>
            {
                Console.WriteLine("result:" + result);
                are.Set();
            }, error =>
            {
                Console.WriteLine("error:" + error);
                are.Set();
            }, (long a, long b) =>
            {
                Console.WriteLine(a + "-" + b);
            });

            Thread.Sleep(3000);
            hresult.Abort();

            hresult.WaitForEnd();

            are.WaitOne();
        }
    }
}
