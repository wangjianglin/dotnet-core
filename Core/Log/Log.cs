using AD.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;

namespace AD.Core.Log
{
    /// <summary>
    /// 用心记录日志信息
    /// </summary>
    public class Log
    {
        private string name;
        private Log(string name)
        {
            this.name = name;
        }

        public static long _LogFileSize;
        /// <summary>
        /// 日志文件大小，以字节为单位
        /// </summary>
        [DefaultValue(5 * 1024 * 1024)]
        public static long LogFileSize
        {
            get { return _LogFileSize; }
            set
            {
                _LogFileSize = value;
                if (_LogFileSize < 1 * 1025 * 1025)//不能小于1M
                {
                    _LogFileSize = 1 * 1024 * 1024;
                }
            }
        }
        /// <summary>
        /// 记录日志文件
        /// </summary>
        /// <param name="log">日志信息</param>
        /// <param name="level">日志级别(默认为正常信息)</param>
        public void LogInfo(string logString, Level level = Level.INFO)
        {
            int _level = (int)AD.Core.ViewModel.Context.Global.LogLevel;
            int l = (int)level;
            if (l < _level)
            {
                return;
            }
            logString = level.ToString() + "：     " + logString + "\r\n";
            log(logString, name);
        }

        /// <summary>
        /// 记录正常日志文件信息
        /// </summary>
        /// <param name="info"></param>
        public void Info(string info)
        {
            LogInfo(info, Level.INFO);
        }

        /// <summary>
        /// 记录警告日志文件信息
        /// </summary>
        /// <param name="warning"></param>
        public void Warning(string warning)
        {
            LogInfo(warning, Level.WARNING);
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="debug"></param>
        public void Debug(string debug)
        {
            LogInfo(debug, Level.DEBUG);
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="error"></param>
        public void Error(string error)
        {
            LogInfo(error, Level.ERROR);
        }

        private static Stream logStream = null;
        private static FileInfo logFile = null;

        private static object lockObj = new object();

        /// <summary>
        /// 打开文件流，并将异常信息记录到日志文件中
        /// </summary>
        private void OpenStream(string name)
        {
            DirectoryInfo info = new DirectoryInfo("log");
            if (!info.Exists)
            {
                info.Create();
            }
            int count = 0;
            FileInfo[] files = info.GetFiles();
            // 查找日志文件夹下是否存在提示框异常记录的日志文件

            // logStream为空则没有创建，重新创建并打开流
            if (logStream == null)
            {
                lock (lockObj)
                {
                    if (logStream == null)
                    {
                        logFile = new FileInfo("log/" + name + ".log");
                        if (!logFile.Exists)
                        {
                            logStream = logFile.Create();
                            logStream.Close();
                            logFile = new FileInfo("log/" + name + ".log");
                        }
                        else
                        {
                            // 存在日志文件则追加写入内容
                            if (logFile.Length > 5 * 1024 * 1024)
                            {
                                lock (lockObj)
                                {
                                    // 判断当前文件大小是否大于 5M
                                    if (logFile.Length > 5 * 1024 * 1024)
                                    {
                                        foreach (FileInfo fileItem in files)
                                        {
                                            if (fileItem.FullName.Contains(name))
                                            {
                                                count++;
                                            }
                                        }

                                        string logFileName = name + "_" + (count + 1) + ".log";
                                        logFile.MoveTo("log/" + logFileName); //如果大于5M， 将原有的日志内容移到新的日志文件中并创建一个新的日志文件
                                        logFile = new FileInfo("log/" + name + ".log");
                                    }
                                }
                            }
                        }
                        logStream = logFile.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
                    }
                }
            }
            else
            {
                // 存在日志文件则追加写入内容
                if (logFile.Length > 5 * 1024 * 1024)
                {
                    lock (lockObj)
                    {
                        // 判断当前文件大小是否大于 5M
                        if (logFile.Length > 5 * 1024 * 1024)
                        {
                            foreach (FileInfo fileItem in files)
                            {
                                if (fileItem.FullName.Contains(name))
                                {
                                    count++;
                                }
                            }

                            string logFileName = name + "_" + (count + 1) + "log";
                            logStream.Close();
                            logFile.MoveTo(logFileName); //如果大于5M， 将原有的日志内容移到新的日志文件中并创建一个新的日志文件
                            logFile = new FileInfo("log/" + name + ".log");
                            logStream = logFile.Create();
                            logFile = new FileInfo("log/" + name + ".log");
                            logStream = logFile.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 提示框显示的异常信息记录到日志文件中
        /// </summary>
        /// <param name="obj"></param>
        private void log(string obj, string name)
        {
            //打开文件
            OpenStream(name);

            //把日志写到 文件
            byte[] bs = System.Text.Encoding.Default.GetBytes(obj);

            logStream.Write(bs, 0, bs.Count());
            logStream.Flush();
        }

        private static IDictionary<string, Log> dict = new Dictionary<string, Log>();
        private static object dictLock = new object();

        /// <summary>
        /// 保存日志对象集合
        /// </summary>
        public static PropertyIndex<string, Log> Logs = new PropertyIndex<string, Log>(name =>
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
                Log log = new Log(name);
                dict.Add(name, log);
                return log;
            }
        }, (key, value) =>
        {
        });

        //public static Log HttpLog = new Log("http.communicate");
        public static Log HttpLog = Logs["http.communicate"];

        public static Log DebugLog = Logs["debug"];

        //public static Log Log = new Log("log");
        //public static Log Log = Logs["log"];
        //public static Log Log = new Log();
    }
}
