using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.IO;
using System.Globalization;
using Lin.Comm.Http.Packages;
using Lin.Comm.Http;


namespace AD.Test.Core
{

    public class ExceptionPackage : Package
    {
        public ExceptionPackage()
        {
            this.location = "/cloud/action/comm!exception.action";
            this.RespType = typeof(object);
            base.Version.Major = 0u;
            base.Version.Minor = 0u;
        }
        public override IDictionary<string, object> GetParams()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            return param;
        }
    }

    public class AdExceptionPackage : Package
    {
        public AdExceptionPackage()
        {
            this.location = "/cloud/action/comm!adException.action";
            this.RespType = typeof(object);
            base.Version.Major = 0u;
            base.Version.Minor = 0u;
        }
    }



    internal class A
    {
        //private string name = "A";
        public A()
        {
            HttpCommunicate.HttpRequest += (HttpCommunicateType type, object content) =>
            {
                //Console.WriteLine("-------------0"+name);
                Console.WriteLine("-------------0");
            };
        }
        ~A()
        {
            Console.WriteLine("~A---------------");
        }
    }
    [TestClass]
    public class HttpTest:BaseTest
    {
        [TestMethod]
        public void TestFinally()
        {
            try
            {
                //try
                //{
                //    throw new Exception();
                //}
                //finally
                //{
                //    global::System.Console.WriteLine("finally.");
                //}
                Lin.Util.ActionExecute.Execute(() =>
                {
                    global::System.Console.WriteLine("action1.");
                    throw new Exception();
                }, () =>
                {
                    global::System.Console.WriteLine("action2.");
                    throw new Exception();
                }, () =>
                {
                    global::System.Console.WriteLine("action3.");
                    throw new Exception();
                });
            }
            catch (Exception e)
            {
                global::System.Console.WriteLine("exception.");
                global::System.Console.WriteLine(e);
                global::System.Console.WriteLine("-------------------------------------.");
                global::System.Console.WriteLine(e.InnerException);
            }
        }

        public void Action()
        {
            A a = new A();
            HttpCommunicate.HttpRequest += (HttpCommunicateType type, object content) =>
            {
                Console.WriteLine("-------------1");
            };
        }

        public void Action(Action a)
        {
            Console.WriteLine("action:" + a.GetHashCode());
        }
        public event Action a;
        [TestMethod]
        public void TestHttpCommunicate()
        {
            //Action(Action);
            //Action(Action);
            //Action(Action);
            //Action();
            //System.GC.Collect();
            //System.GC.Collect();
            //System.GC.Collect();
            Lin.Comm.Http.HttpCommunicate.CommUri = new Uri("http://localhost:8080/web/__http_comm_protocol__");
            Lin.Comm.Http.HttpCommunicate.CommUri = new Uri("http://localhost:8080/");
            HttpCommunicate.HttpRequest += (HttpCommunicateType type, object content) =>
            {
                Console.WriteLine("-------------2");
            };
            object o = (object)null;
            Console.WriteLine("obj:" + o);
            for (int n = 0; n < 10; n++)
            {
                TestPackage package = new TestPackage();
                package.data = "测试中文--2";
                Request(package, (result, warning) =>
                {
                    Console.WriteLine("result:" + result);
                    Assert.IsTrue(package.data.Equals(result));
                }, error =>
                {
                    Console.WriteLine("error:" + error.cause);
                }, 0);
            }
        }

        [TestMethod]
        public void TestHttpException()
        {
            ExceptionPackage package = new ExceptionPackage();
            Request(package, (result, warning) =>
            {
                Console.WriteLine("result:" + result);
            }, error =>
            {
                Console.WriteLine("error:" + error.cause);
            }, -1);
        }
        [TestMethod]
        public void TestHttpAdException()
        {
            AdExceptionPackage package = new AdExceptionPackage();
            Request(package, (result, warning) =>
            {
                Console.WriteLine("result:" + result);
            }, error =>
            {
                Console.WriteLine("error:" + error.cause);
            }, -0x10101L);
        }
        [TestMethod]
        public void TestDateTime()
        {
            DateTime date = DateTime.ParseExact("2008-10-01 16:44:12.000", "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.CreateSpecificCulture("en-US"));
            Console.WriteLine("date:" + date);
            //IFormatProvider format = new CultureInfo("yyyy-MM-dd HH:mm:ss");
            //date = DateTime.ParseExact("2012-07-11 17:29:23", "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture,DateTimeStyles.None);
            date = DateTime.ParseExact("2012-07-11 17:29:23", "yyyy-MM-dd HH:mm:ss",null);
            Console.WriteLine("date:" + date);
        }

        [TestMethod]
        public void TestJson()
        {
            string json = "eyJlcnJvciI6LTY1NTM3LCJzZXF1ZXVlaWQiOjAsIndhcm5pbmciOltdLCJyZXN1bHQiOnsiYWN0%0D%0AaW9uRXJyb3JzIjpbXSwiYWN0aW9uTWVzc2FnZXMiOltdLCJlcnJvcnMiOnt9LCJmaWVsZEVycm9y%0D%0AcyI6e319fQ%3D%3D%0D%0A";
            json = HttpUtility.UrlDecode(json);

            //resultString new String(System.Convert.FromBase64String(resultString));
            Stream stream = new MemoryStream(global::System.Convert.FromBase64String(json));
            json = new StreamReader(stream, Encoding.UTF8, true).ReadToEnd();
            Console.WriteLine("json:" + json);
        }
       
    }
}
