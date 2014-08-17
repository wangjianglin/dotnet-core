using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lin.Core.Cache;
using System.IO;
using System.Windows;
using Lin.Core.Log;
using Lin.Plugin;

namespace Lin.Core.ViewModel2
{
    /// <summary>
    /// 记录应用程序上下文
    /// </summary>
    [Serializable]
    public class Context : ViewModel, IContext
    {
        private static LogLevel _level = LogLevel.INFO; // 日志级别默认值

        public LogLevel LogLevel
        {
            get { return _level; }
        }

        /// <summary>
        /// 是否有网络链接(及是否登录或单击版)
        /// </summary>
        private bool isNet = false;

        public bool IsNet
        {
            get
            {
                return isNet;
            }
            set
            {
                this.isNet = value;
                this.OnPropertyChanged("IsNet");
            }
        }

        /// <summary>
        /// 全局应用程序属性改变
        /// </summary>
        /// <param name="s">属性名称</param>
        /// <returns></returns>
        //public object this[string s]
        //{
        //    get { return this.Property[s]; }
        //    set
        //    {
        //        if (s == "user.accountNumber")
        //        {
        //            if (value != null)
        //            {
        //                this.IsNet = true;
        //            }
        //            else
        //            {
        //                this.IsNet = false;
        //            }
        //        }
        //        this.Property[s] = value;
        //        this.OnPropertyChanged("");
        //    }
        //}

        /// <summary>
        /// 用于记录应用程序的全局信息
        /// </summary>
        private static object icontextProxyLock = new object();
        private static IContext _contextProxyInstance = null;

        public static IContext Global
        {
            get
            {
                if (_contextProxyInstance == null)
                {
                    lock (icontextProxyLock)
                    {
                        if (_contextProxyInstance == null)
                        {
                            _contextProxyInstance = new ContextProxy();
                        }
                    }
                }

                return _contextProxyInstance;
            }
        }

        #region 全局对象跨域

        private IList<ListenerEvent> listeners = new List<ListenerEvent>();
        internal void AddListener(ListenerEvent listener)
        {
            listeners.Add(listener);
        }

        /// <summary>
        /// 当Context对象中属性发生改变后，发布到ContextProxy对象中处理
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged(string propertyName)
        {
            foreach (ListenerEvent listener in listeners)
            {
                listener.Fire(propertyName);
            }
        }

        private static object lockObj = new object();
        private static Context _GlobalImpl;
        internal static Context GlobalImpl
        {
            get
            {
                if (_GlobalImpl == null)
                {
                    lock (lockObj)
                    {
                        if (_GlobalImpl == null)
                        {
                            _GlobalImpl = new Context();
                        }
                    }
                }
                return _GlobalImpl;
            }
        }

        #endregion

        public static CacheProxy Cache; // 缓存代理对象
        public static DirectoryInfo CachePath; // 存放缓存的路径

        static Context()
        {
            Lin.Core.Config.IConfigManager Config;
            Config = Lin.Core.Config.ConfigManager.System;
            if (string.IsNullOrWhiteSpace(Config["CachePath"]))
            {
                CachePath = new DirectoryInfo(Environment.CurrentDirectory + "\\Data\\Cache");
                Cache = new CacheProxy(CachePath);
                Config["CachePath"] = CachePath.FullName;
            }
            else
            {
                CachePath = new DirectoryInfo(Config["CachePath"].ToString());
                Cache = new CacheProxy(CachePath);
            }

            // 读取命令行参数的日志级别值
            IList<string> logLevel = CommandLineArguments.Args["Log"];
            if (logLevel != null && logLevel.Count > 0)
            {
                _level = (LogLevel)Enum.Parse(typeof(LogLevel), logLevel[0], true);
            }
        }
    }
}
