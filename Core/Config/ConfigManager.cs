using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Configuration;
using System.Windows;
using Lin.Util.Json;

namespace Lin.Core.Config
{
    /// <summary>
    /// 实现对配置文件的读取
    /// </summary>
    [Serializable]
    public class ConfigManager : INotifyPropertyChanged, IConfigManager
    {
        #region 写入或读取配置文件

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileSectionNames(StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileSection(string section, byte[] retVal, int size, string filePath);

        /// <summary>  
        /// 获取所有节点名称(Section)  
        /// </summary>  
        /// <param name="lpszReturnBuffer">存放节点名称的内存地址,每个节点之间用\0分隔</param>  
        /// <param name="nSize">内存大小(characters)</param>  
        /// <param name="lpFileName">Ini文件</param>  
        /// <returns>内容的实际长度,为0表示没有内容,为nSize-2表示内存大小不够</returns>  
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, uint nSize, string lpFileName);

        /// <summary>
        /// 将信息写入文件
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        private static void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, IniPath);
        }

        /// <summary>
        /// 从ini文件中读取数据
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private static string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            int i = GetPrivateProfileString(Section, Key, "", temp, 1024, IniPath);
            return temp.ToString();
        }
        #endregion

        static ConfigManager()
        {
            string iniDir = Environment.CurrentDirectory;
            string configDir = ConfigurationManager.AppSettings["configDir"];

            if (!string.IsNullOrEmpty(configDir))
            {
                if (configDir.StartsWith("."))
                {
                    iniDir += "/" + configDir;
                }
                else
                {
                    iniDir = configDir;
                }
            }

            string configFileName = "config.ini";
            if (ConfigurationManager.AppSettings["configName"] != null)
            {
                configFileName = ConfigurationManager.AppSettings["configName"];
            }
            IniPath = iniDir + "\\" + "Data" + "\\config\\" + configFileName;
            Net = GetConfigManager("Net");
            System = GetConfigManager("System");
            Advanced = GetConfigManager("Advanced");
            AdowsTest = GetConfigManager("AdowsTest");
            FileInfo file = new FileInfo(IniPath);
            if (!file.Exists)
            {
                DirectoryInfo dir;

                dir = new DirectoryInfo(iniDir + "\\" + "Data");
                if (!dir.Exists)
                {
                    dir.Create();
                }
                dir = new DirectoryInfo(iniDir + "\\" + "Data" + "\\config");
                if (!dir.Exists)
                {
                    dir.Create();
                }
                FileStream stream = file.Create();
                stream.Close();
                string[] keys = ConfigurationManager.AppSettings.AllKeys;
                foreach (string key in keys)
                {
                    if (key.Contains('#'))
                    {
                        string[] name = key.Split('#');
                        ConfigManager.IniWriteValue(name[0], name[1], ConfigurationManager.AppSettings[key]);
                    }
                }
            }
        }

        internal ConfigManager(string section)
        {
            this.section = section;
        }

        #region Section数组，Section下所有的值和名称

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private static readonly string IniPath;
        private static string[] GetStringsFromBuffer(byte[] buffer, int len)
        {
            string temp;
            if (len <= 0)
            {
                temp = global::System.Text.Encoding.Default.GetString(buffer, 0, len);
            }
            else
            {
                temp = global::System.Text.Encoding.Default.GetString(buffer, 0, len - 1);
            }
            temp = temp.TrimEnd();
            return temp.Split('\0');

        }

        public string[] Section
        {
            get
            {
                uint MAX_BUFFER = 2048;

                string[] sections = new string[0];      //返回值  
                try
                {
                    //申请内存  
                    IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));
                    uint bytesReturned = GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, IniPath);
                    if (bytesReturned != 0)
                    {
                        //读取指定内存的内容  
                        string local = Marshal.PtrToStringAuto(pReturnedString, (int)bytesReturned).ToString();

                        //每个节点之间用\0分隔,末尾有一个\0  
                        sections = local.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    //释放内存  
                    Marshal.FreeCoTaskMem(pReturnedString);
                }
                catch (Exception)
                {

                }

                return sections;
            }
        }
        /// <summary>
        /// 返回当前Section下的所有的值
        /// </summary>
        public string[] Values
        {
            get
            {
                byte[] bs = new byte[2048];
                int i = GetPrivateProfileSection(this.section, bs, bs.Length, IniPath);
                int preI = 0;
                while (i != preI)
                {
                    bs = new byte[bs.Length + 1024];
                    preI = i;
                    i = GetPrivateProfileSection(this.section, bs, bs.Length, IniPath);
                }
                string[] tmp = GetStringsFromBuffer(bs, preI);
                if (tmp == null || tmp.Length == 0)
                {
                    return tmp;
                }
                string[] values = new string[tmp.Length];
                for (int n = 0; n < tmp.Length; n++)
                {
                    values[n] = tmp[n].Split('=')[1];
                }
                return values;
            }
        }

        /// <summary>
        /// 返回当前Section下的所有项的名称
        /// </summary>
        public string[] Names
        {
            get
            {
                byte[] bs = new byte[2048];
                int i = GetPrivateProfileSection(this.section, bs, bs.Length, IniPath);
                int preI = 0;
                while (i != preI)
                {
                    bs = new byte[bs.Length + 1024];
                    preI = i;
                    i = GetPrivateProfileSection(this.section, bs, bs.Length, IniPath);
                }
                string[] tmp = GetStringsFromBuffer(bs, preI);
                if (tmp == null || tmp.Length == 0)
                {
                    return tmp;
                }
                string[] names = new string[tmp.Length];
                for (int n = 0; n < tmp.Length; n++)
                {
                    names[n] = tmp[n].Split('=')[0];
                }
                return names;
            }
        }

        /// <summary>
        /// 读取和写入当前Section指定name的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                if (this.Names.Contains(name))
                {
                    return IniReadValue(this.section, name);
                }
                return null;
            }
            set
            {
                IniWriteValue(this.section, name, value);
                this.OnPropertyChanged(name);
            }
        }

        #endregion

        public static IConfigManager Net { get; private set; }

        public static IConfigManager System { get; private set; }

        public static IConfigManager Advanced { get; private set; }

        public static IConfigManager AdowsTest { get; private set; }

        /// <summary>
        /// 根据指定的section名称，返回配置对象
        /// </summary>
        /// <param name="section">名称</param>
        /// <returns>配置对象</returns>
        private string section;
        private static object GetLock = new object();
        private static Dictionary<string, IConfigManager> AllConfig = new Dictionary<string, IConfigManager>();
        public static IConfigManager GetConfigManager(string section)
        {
            if (AllConfig.ContainsKey(section))
            {
                return AllConfig[section];
            }
            lock (GetLock)
            {
                if (AllConfig.ContainsKey(section))
                {
                    return AllConfig[section];
                }
                else
                {
                    ConfigManagerProxy tmp = new ConfigManagerProxy(section);
                    AllConfig.Add(section, tmp);
                    return tmp;
                }
            }
        }

        /// <summary>
        /// 根据指定的section名称，返回配置对象
        /// </summary>
        /// <param name="section">名称</param>
        /// <returns>配置对象</returns>
        private static Dictionary<string, ConfigManager> ProxyAllConfig = new Dictionary<string, ConfigManager>();
        internal static ConfigManager ProxyGetConfigManager(string section)
        {
            if (ProxyAllConfig.ContainsKey(section))
            {
                return ProxyAllConfig[section];
            }
            lock (GetLock)
            {
                if (AllConfig.ContainsKey(section))
                {
                    return ProxyAllConfig[section];
                }
                else
                {
                    ConfigManager tmp = new ConfigManager(section);
                    ProxyAllConfig.Add(section, tmp);
                    return tmp;
                }
            }
        }

        /// <summary>
        /// 写入Section下指定名称Name的值
        /// </summary>
        /// <param name="section">配置文件中Section名称(如：Net,System等)</param>
        /// <param name="name">属性名称</param>
        /// <param name="value">值</param>
        public static void Config(string section, string name, string value)
        {
            IniWriteValue(section, name, value);
        }

        /// <summary>
        /// 写入Section下指定名称Name的值
        /// </summary>
        /// <param name="section">配置文件中Section名称(如：Net,System等)</param>
        /// <param name="name">属性名称</param>
        public static string Config(string section, string name)
        {
            return IniReadValue(section, name);
        }

        /// <summary>
        /// 添加监听器到集合里，到CongfigManagerProxy代理中循环执行
        /// </summary>
        /// <param name="listener"></param>
        private IList<ListenerEvent> listeners = new List<ListenerEvent>();
        internal void AddListener(ListenerEvent listener)
        {
            listeners.Add(listener);
        }

        /// <summary>
        /// 当Context对象中属性发生改变后，发布到ContextProxy对象中处理
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            foreach (ListenerEvent listener in listeners)
            {
                listener.Fire(propertyName);
            }
        }

        #region 以下实现INotifyPropertyChanged接口，此对象可以作为数据绑定时的数据源
        #region DisplayName

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        [JsonSkip]
        public virtual string DisplayName { get; protected set; }

        #endregion // DisplayName

        #region Debugging Aides

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged Members

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        #endregion // IDisposable Members
        #endregion
    }
}
