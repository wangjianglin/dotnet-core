using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Lin.Util;
using Lin.Comm.Http;

namespace Lin.Core.Utils
{
    public class ExceptionInfoToString
    {
        /// <summary>
        /// ADException返回String
        /// </summary>
        /// <param name="e"></param>
        /// <param name="pre"></param>
        /// <returns></returns>
        public static string ADExceptionToString(Exception obj)
        {
            if (obj == null)
            {
                return "";
            }
            string result = "";
            if (obj != null)
            {
                result += "DateTime：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                result += "CallBack：" + new StackTrace(true);
                if (obj as LinException != null)
                {
                    LinException e = obj as LinException;
                    if (e.Code != 0)
                    {
                        result += "\r\nExceptionCode：" + e.Code + "\r\n";
                    }
                }
                if (!string.IsNullOrEmpty(obj.Message))
                {
                    result += "ExceptionMessage：" + obj.Message + "\r\n";
                }
                if (!string.IsNullOrEmpty(obj.StackTrace))
                {
                    result += "ExceptionStackTrace：" + obj.StackTrace + "\r\n";
                }
                if (!string.IsNullOrEmpty(obj.Source))
                {
                    result += "ExceptionSource：" + obj.Source + "\r\n";
                }
                if (obj.InnerException != null)
                {
                    result += ExceptionToString(obj.InnerException);
                }
                result = result + "\r\n\r\n\r\n";
            }

            return result;
        }

        /// <summary>
        /// 记录内部异常信息
        /// </summary>
        /// <param name="e"></param>
        /// <param name="pre"></param>
        /// <returns></returns>
        private static string ExceptionToString(Exception e)
        {
            if (e == null)
            {
                return "";
            }
            string result = e.GetType().FullName + "\r\n" + e.Message + "\r\nsource:" + e.Source + "\r\ntarget site:" + e.TargetSite + "\r\nhelp link:" + e.HelpLink + "\r\nstack trace:\r\n" + e.StackTrace;
            if (e.InnerException != null)
            {
                result += ExceptionToString(e.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 讲http请求错误返回String
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string HttpErrorToString(Error error)
        {
            if (error == null)
            {
                return "";
            }
            string result = "";
            result = "DateTime：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                "\r\nStackTrace：" + error.stackTrace + "\r\nErrorCode：" + error.code + "\r\n";
            if (!string.IsNullOrEmpty(error.message))
            {
                result += "ErrorMessage：" + error.message;
            }
            if (!string.IsNullOrEmpty(error.stackTrace))
            {
                result += "ErrorStackTrace：" + error.stackTrace + "\r\n";
            }
            if (!string.IsNullOrEmpty(error.cause))
            {
                result += "ErrorCause：" + error.cause + "\r\n";
            }
            if (error.data != null)
            {
                if (!string.IsNullOrEmpty(error.data.ToString()))
                {
                    result += "ErrorData：" + error.data.ToString() + "\r\n";
                }
            }
            return result;
        }
    }
}
