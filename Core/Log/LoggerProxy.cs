using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using Lin.Core.Utils;
using Lin.Util;

namespace Lin.Core.Log
{
    [Serializable]
    public class LoggerProxy : ILogger
    {
        private string name;
        public LoggerProxy(string name)
        {
            this.name = name;
        }
        /// <summary>
        /// 记录日志文件
        /// </summary>
        /// <param name="log">日志信息</param>
        /// <param name="level">日志级别(默认为正常信息)</param>
        private readonly string logStringKey = "LogRecord_logStringKey_" + DateTime.Now.Ticks;
        private readonly string logLevelKey = "LogRecord_logLevelKey_" + DateTime.Now.Ticks;
        public void LogRecord(string logString, LogLevel level = LogLevel.INFO)
        {
            //AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            //appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + logStringKey, logString);
            //appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + logLevelKey, level);
            //appDomain.DoCallBack(() =>
            //{
            //    object logStr = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + logStringKey);
            //    object logLevel = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + logLevelKey);
            //    if (logStr != null && logLevel != null)
            //    {
            //        string logString1 = logStr.ToString();
            //        LogLevel level1 = (LogLevel)logLevel;
            //        LoggerProxy.Logs[name].LogRecord(logString1, level1);
            //    }
            //});
            //appDomain.SetData(logStringKey, null);
            //appDomain.SetData(logLevelKey, null);
        }


        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="error"></param>
        private readonly string errorStringKey = "LogRecord_errorStringKey_" + DateTime.Now.Ticks;
        public void Error(string error)
        {
            //AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            //appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + errorStringKey, error);
            //appDomain.DoCallBack(() =>
            //{
            //    object obj = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + errorStringKey);
            //    if (obj != null)
            //    {
            //        string error1 = obj.ToString();
            //        LoggerProxy.Logs[name].Error(error1);
            //    }
            //});
            //appDomain.SetData(errorStringKey, null);
        }

        /// <summary>
        /// 记录正常日志文件信息
        /// </summary>
        /// <param name="info"></param>
        private readonly string infoStringKey = "LogRecord_infoStringKey_" + DateTime.Now.Ticks;
        public void Info(string info)
        {
            //AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            //appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + infoStringKey, info);
            //appDomain.DoCallBack(() =>
            //{
            //    object obj = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + infoStringKey);
            //    if (obj != null)
            //    {
            //        string info2 = obj.ToString();
            //        LoggerProxy.Logs[name].Info(info2);
            //    }
            //});
            //appDomain.SetData(infoStringKey, null);
        }

        /// <summary>
        /// 记录警告日志文件信息
        /// </summary>
        /// <param name="warning"></param>
        private readonly string warningStringKey = "LogRecord_warningStringKey_" + DateTime.Now.Ticks;
        public void Warning(string warning)
        {
            //AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            //appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + warningStringKey, warning);
            //appDomain.DoCallBack(() =>
            //{
            //    object obj = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + warningStringKey);
            //    if (obj != null)
            //    {
            //        string warning1 = obj.ToString();
            //        LoggerProxy.Logs[name].Warning(warning1);
            //    }
            //});
            //appDomain.SetData(warningStringKey, null);
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="debug"></param>
        private readonly string debugStringKey = "LogRecord_debugStringKey_" + DateTime.Now.Ticks;
        public void Debug(string debug)
        {
            //AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            //appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + debugStringKey, debug);
            //appDomain.DoCallBack(() =>
            //{
            //    object obj = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + debugStringKey);
            //    if (obj != null)
            //    {
            //        string debug1 = obj.ToString();
            //        LoggerProxy.Logs[name].Debug(debug1);
            //    }
            //});
            //appDomain.SetData(debugStringKey, null);
        }

        /// <summary>
        /// 记录所有的日志对象，根据日志文件名称查找日志对象，并返回
        /// </summary>
        private static IDictionary<string, ILogger> dict = new Dictionary<string, ILogger>();
        private static object dictLock = new object();
        private static IndexProperty<string, ILogger> Logs = new IndexProperty<string, ILogger>(name =>
        {
            if (dict.ContainsKey(name))
            {
                return dict[name];
            }
            lock (dictLock)
            {
                if (dict.ContainsKey(name))
                {
                    return dict[name];
                }
                Logger proxy = new Logger(name);
                dict.Add(name, proxy);
                return proxy;
            }
        }, (key, value) =>
        {
        });

    }
}
