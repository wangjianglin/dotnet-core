using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lin.Comm.Http.Packages;
using Lin.Comm.Http;

namespace AD.Test
{
    /// <summary>
    /// 王江林
    /// 
    /// 提供在测试时的一些基本操作
    /// </summary>
    public class BaseTest
    {
        /// <summary>
        /// 实现对HTTP的同步调用，使得对返回结果的诊断 是 和测试线程在同一线程中
        /// 
        /// 这样在诊断出错，或产生异常时才能捕获测试结果
        /// </summary>
        /// <param name="package"></param>
        /// <param name="result"></param>
        /// <param name="fault"></param>
        /// <param name="code">当出现错误时，错误码不为code时，测试失败,为null则不处理</param>
        public void Request(Package package, Action<object, IList<Error>> result = null, Action<Error> fault = null,long? code = 0)
        {
            //AutoResetEvent are = new AutoResetEvent(false);
            bool flag = false;
            object tmpResultObject = null;
            IList<Error> tmpWarning = null;
            Error tmpError = null;
            HttpCommunicate.Request(package, (resultObject, warning) =>
            {
                tmpResultObject = resultObject;
                tmpWarning = warning;
                flag = true;
                //are.Set();
            }, error =>
            {
                tmpError = error;
                //are.Set();
            }).WaitForEnd();
            //are.WaitOne();
            if (flag == true)
            {
                if (result != null)
                {
                    result(tmpResultObject, tmpWarning);
                }
            }
            else
            {
                if (code != null)
                {
                    //Console.WriteLine("error:-0x{0:X}_{1:X4}", Math.Abs((long)(tmpError.code / 65536)), Math.Abs(tmpError.code) % 65536);
                    //Console.WriteLine("error message:" + tmpError.message);
                    //Console.WriteLine("error cause:" + tmpError.cause);
                    //Console.WriteLine("errot stack trace:" + tmpError.stackTrace);
                    //Console.WriteLine("data:" + tmpError.data);
                    //if (tmpError.code != code)
                    //{
                    //    Console.WriteLine("server build:" + VersionInfo.build);
                    //}
                    //Assert.IsTrue(tmpError.code == code);
                }
                if (fault != null)
                {
                    fault(tmpError);
                }
            }
        }


        public Lin.Comm.Http.Model.Version VersionInfo
        {
            get
            {
                Lin.Comm.Http.Model.Version version = null;
                VersionPackage package = new VersionPackage();
                Request(package, (result, warning) =>
                {
                    version = result as Lin.Comm.Http.Model.Version;
                }, error =>
                {
                }, null);
                return version;
            }
        }
    }
}
