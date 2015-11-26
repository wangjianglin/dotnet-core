using Lin.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Lin.Util;

namespace Lin.Core.Log
{
    /// <summary>
    /// 用心记录日志信息
    /// </summary>
    [Serializable]
    public class Logger : ILogger
    {
        private string name;
        internal Logger(string name)
        {
            this.name = name;
            this.sourceFileName = name + "_" + DateTime.Now.ToString("yyyy-MM-dd");
            logFile = new FileInfo(dir + "\\log\\" + sourceFileName + ".log");
        }

        static Logger()
        {
            LogFileSize = 5 * 1024 * 1024;
        }

        /// <summary>
        /// 目录
        /// </summary>
        public static DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
        public static DirectoryInfo Dir
        {
            get { return dir; }
            set
            {
                dir = value;
            }
        }

        public static long _LogFileSize;
        /// <summary>
        /// 日志文件大小，以字节为单位
        /// </summary>
        public static long LogFileSize
        {
            get { return _LogFileSize; }
            set
            {
                _LogFileSize = value;
                if (_LogFileSize < 1 * 1024 * 1024)//不能小于1M
                {
                    _LogFileSize = 1 * 1024 * 1024;
                }
            }
        }

        private DateTime logDateTime;
        /// <summary>
        /// 记录日志文件
        /// </summary>
        /// <param name="log">日志信息</param>
        /// <param name="level">日志级别(默认为正常信息)</param>
        public void LogRecord(string logString, LogLevel level = LogLevel.INFO)
        {
            int _level = (int)Lin.Core.Context.Global.LogLevel;
            int l = (int)level;
            if (l < _level)
            {
                return;
            }
            logDateTime = DateTime.Now;
            logString = level.ToString() + "     " + logDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "：     " + logString + "\r\n";
            log(logString);
        }

        /// <summary>
        /// 记录正常日志文件信息
        /// </summary>
        /// <param name="info"></param>
        public void Info(string info)
        {
            LogRecord(info, LogLevel.INFO);
        }

        /// <summary>
        /// 记录警告日志文件信息
        /// </summary>
        /// <param name="warning"></param>
        public void Warning(string warning)
        {
            LogRecord(warning, LogLevel.WARNING);
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="debug"></param>
        public void Debug(string debug)
        {
            LogRecord(debug, LogLevel.DEBUG);
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="error"></param>
        public void Error(string error)
        {
            LogRecord(error, LogLevel.ERROR);
        }

        private Stream logStream = null;
        private FileInfo logFile = null;
        private string sourceFileName = "";

        /// <summary>
        /// 当要写入内容的文件大于5M时，移动到一个新的文件里去
        /// </summary>
        private void moveFile()
        {
            int n = 0;
            while ((new FileInfo(dir + "\\log\\" + sourceFileName + "_" + ++n + ".log").Exists)) ;
            logFile.MoveTo(dir + "\\log\\" + sourceFileName + "_" + n + ".log");
            logFile = new FileInfo(dir + "\\log\\" + sourceFileName + ".log");
        }

        /// <summary>
        /// 往文件里追加内容，或创建一个新的文件
        /// </summary>
        private void createFileStream()
        {
            logFile = new FileInfo(dir + "\\log\\" + sourceFileName + ".log");
            if (logFile.Exists)
            {
                logStream = logFile.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            }
            else
            {
                logStream = logFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                logFile = new FileInfo(dir + "\\log\\" + sourceFileName + ".log");
            }
        }
        /// <summary>
        /// 打开文件流，并将异常信息记录到日志文件中
        /// </summary>
        private void OpenStream()
        {
            lock (lockObj)
            {
                try
                {
                    OpenStreamImpl();
                }
                catch (Exception)
                {
                    logStream = null;
                    OpenStreamImpl();
                }
            }
        }

        /// <summary>
        /// 创建文件流
        /// </summary>
        private void OpenStreamImpl()
        {
            string fileName = name + "_" + logDateTime.ToString("yyyy-MM-dd");
            if (fileName != sourceFileName)
            {
                logStream = null;
                sourceFileName = fileName;
            }

            logFile = new FileInfo(dir + "\\log\\" + fileName + ".log");
            if (logStream == null)
            {
                DirectoryInfo info = new DirectoryInfo(dir + "\\log\\");
                if (!info.Exists)
                {
                    info.Create();
                }

                if (logFile.Exists)
                {
                    // 存在日志文件则追加写入内容
                    if (logFile.Length > LogFileSize)
                    {
                        moveFile();
                    }
                }
                createFileStream();
            }
            else
            {
                // 存在日志文件则追加写入内容
                if (logFile.Length > LogFileSize)
                {
                    logStream.Close();
                    moveFile();
                    createFileStream();
                }
            }
        }

        /// <summary>
        /// 提示框显示的异常信息记录到日志文件中
        /// </summary>
        /// <param name="obj"></param>
        private void log(string obj)
        {
            try
            {
                //打开文件
                OpenStream();

                //把日志写到 文件
                byte[] bs = System.Text.Encoding.Default.GetBytes(obj);

                logStream.Write(bs, 0, bs.Count());
                logStream.Flush();
            }
            catch
            {
                Console.Error.Write("Writting Logger Error!");
            }
        }

        private static IDictionary<string, ILogger> dict = new Dictionary<string, ILogger>();
        private static object dictLock = new object();
        private static object lockObj = new object();

        /// <summary>
        /// 保存日志对象集合
        /// </summary>
        public static IndexProperty<string, ILogger> Logs = new IndexProperty<string, ILogger>(name =>
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
                LoggerProxy proxy = new LoggerProxy(name);
                dict.Add(name, proxy);
                return proxy;
            }
        }, (key, value) =>
        {
        });
    }
}
