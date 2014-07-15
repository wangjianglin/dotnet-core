using Lin.Plugin.AddIn;
using Lin.Plugin.MenuStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lin.Plugin
{
    /// <summary>
    /// 获取插件类
    /// </summary>
    [Serializable]
    public class Plugins
    {
        private DirectoryInfo PluginPath;
        private List<Plugin> _plugins = new List<Plugin>();

        private void AddPlugin(Plugin plugin)
        {
            _plugins.Add(plugin);
            foreach (Plugin p in _plugins)
            {
                p.Plugins = this;
            }
        }
        /// <summary>
        /// 返回所有获取单个插件对象
        /// </summary>
        public IList<Plugin> Pluginss
        {
            get
            {
                return this._plugins.AsReadOnly();
            }
        }

        /// <summary>
        /// 返回组件数据结构
        /// </summary>
        public List<ComponentStructure> ComponentStructures
        {
            get
            {
                List<ComponentStructure> componentCollection = new List<ComponentStructure>();
                foreach (Plugin p in Pluginss)
                {
                    if (p.DefaultLoad)
                    {
                        componentCollection.AddRange(p.ComponentStructures);
                    }
                }
                return componentCollection;
            }
        }
        /// <summary>
        /// 返回早先视图组件数据结构
        /// </summary>
        public List<ViewToken> ViewTokens
        {
            get 
            {
                List<ViewToken> viewtokens = new List<ViewToken>();
                foreach (Plugin p in Pluginss)
                {
                    if (p.DefaultLoad)
                    {
                        viewtokens.AddRange(p.ViewTokens);
                    }
                }
                return viewtokens;
            }
        }

        /// <summary>
        /// 按照版本号排序
        /// </summary>
        private class StringSort : System.Collections.Generic.IComparer<int[]>
        {
            public int Compare(int[] xs, int[] ys)
            {
                if(xs[0]>ys[0] ||
                    (xs[0] == ys[0] && xs[1] > ys[1]) ||
                    (xs[0] == ys[0] && xs[1] == ys[1] && xs[2] > ys[2]) ||
                    (xs[0] == ys[0] && xs[1] == ys[1] && xs[2] == ys[2] && xs[3] > xs[4]))
                {
                    return -1;
                }

                if (xs[0] == ys[0] && xs[1] == ys[1] && xs[2] == ys[2] && xs[3] == ys[3])
                {
                    return 0;
                }
                return 1;
            }
        }

        
        private void Init()
        {
            IList<DirectoryInfo> AllPluginDir = this.PluginPath.GetDirectories().ToList();
            Dictionary<string, Dictionary<int[], DirectoryInfo>> plugins = new Dictionary<string, Dictionary<int[], DirectoryInfo>>();
            foreach (DirectoryInfo item in AllPluginDir)
            {
                string name = item.Name.Split('_')[0];
                string version = item.Name.Split('_')[1];
                string[] versionNumber = version.Split('.');
                int[] number = new int[4];
                if (versionNumber.Length > 2)
                {
                    number[0] = int.Parse(versionNumber[0]);
                    number[1] = int.Parse(versionNumber[1]);
                    string revise = versionNumber[2].Replace("build", ".");
                    number[2] = int.Parse(revise.Split('.')[0]);
                    number[3] = int.Parse(revise.Split('.')[1]);
                }
                else
                {
                    number[0] = int.Parse(versionNumber[0]);
                    number[1] = int.Parse(versionNumber[1]);
                    number[2] = int.MinValue;
                    number[3] = int.MinValue;
                }
                if (plugins.Keys.Contains(name))
                {
                    plugins[name].Add(number, item);
                }
                else
                {
                    plugins.Add(name, new Dictionary<int[], DirectoryInfo>());
                    plugins[name].Add(number, item);
                }
            }

            StringSort sort = new StringSort();
            foreach (string name in plugins.Keys)
            {
                Dictionary<int[], DirectoryInfo> plugin = plugins[name];
                int[][] versions = plugin.Keys.ToArray();
                Array.Sort(versions, sort);
                List<string> ver = new List<string>();
                bool isLoad = false;
                for (int i = 0; i < versions.GetLength(0); i++)
                {
                    foreach (int[] version in plugin.Keys)
                    {
                        if (versions[i] != version)
                        {
                            continue;
                        }
                        string MajorVersion = version[0].ToString() + "." + version[1].ToString();
                        string AllVersion = null;
                        if (version[2] > 0 && version[3] > 0)
                        {
                            AllVersion = version[0].ToString() + "." + version[1].ToString() + "." + version[2].ToString() + "." + version[3].ToString();
                        }
                        if (!ver.Contains(MajorVersion))
                        {
                            if (!isLoad)
                            {
                                Plugin pl = CreatePlugin(plugin[version], this, name, version, true);
                                if (pl != null)
                                {
                                    this.AddPlugin(pl);
                                }
                                isLoad = true;
                            }
                            else
                            {
                                Plugin pl = CreatePlugin(plugin[version], this, name, version, false);
                                if (pl != null)
                                {
                                    this.AddPlugin(pl);
                                }
                            }
                            ver.Add(MajorVersion);
                        }
                    }
                }
            }


            #region
            //FileInfo[] files = this.PluginPath.GetFiles("*.plugin");
            //if (files.Length > 0)
            //{
            //    for (int i = 0; i < files.Length; i++)
            //    {
            //        if (files[i].Name.ToLower() == "Plugin.plugin")
            //        {
            //            StreamReader sr = null;
            //            try
            //            {
            //                sr = new StreamReader(files[i].OpenRead(), Encoding.GetEncoding("utf-8"));
            //                String line = null;
            //                String[] lines = null;
            //                string name = null;
            //                Dictionary<string, string> plugin = new Dictionary<string, string>();
            //                while (!sr.EndOfStream)
            //                {
            //                    line = sr.ReadLine();
            //                    if (line == null || line == "")
            //                    {
            //                        continue;
            //                    }
            //                    lines = line.Split('=');
            //                    if (lines[0] == "name")
            //                    {
            //                        name = lines[1];
            //                        plugin.Add(name, null);
            //                    }
            //                    else
            //                    {
            //                        plugin[name] = lines[1];
            //                    }
            //                }

            //                sr.Close();

            //                foreach (string key in plugin.Keys)
            //                {
            //                    Plugin pl = null;
            //                    string dir = plugin[key];
            //                    if (dir[0] == '.')
            //                    {
            //                        string starpath = Environment.CurrentDirectory;
            //                        DirectoryInfo plugindir = new DirectoryInfo(starpath + "\\" + dir);
            //                        string pluginName = plugindir.Name.Split('_')[0];
            //                        string version = plugindir.Name.Split('_')[1];
            //                        string[] versionNumber = version.Split('.');
            //                        int[] number = new int[4];
            //                        if (versionNumber.Length > 2)
            //                        {
            //                            number[0] = int.Parse(versionNumber[0]);
            //                            number[1] = int.Parse(versionNumber[1]);
            //                            string revise = versionNumber[2].Replace("build", ".");
            //                            number[2] = int.Parse(revise.Split('.')[0]);
            //                            number[3] = int.Parse(revise.Split('.')[1]);
            //                            pl = CreatePlugin(plugindir, this, pluginName, number, true);
            //                            if (pl != null)
            //                            {
            //                                this.AddPlugin(pl);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            number[0] = int.Parse(versionNumber[0]);
            //                            number[1] = int.Parse(versionNumber[1]);
            //                            number[2] = int.MinValue;
            //                            number[3] = int.MinValue;
            //                            pl = CreatePlugin(plugindir, this, pluginName, number, true);
            //                            if (pl != null)
            //                            {
            //                                this.AddPlugin(pl);
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        DirectoryInfo plugindir = new DirectoryInfo(dir);
            //                        string pluginName = plugindir.Name.Split('_')[0];
            //                        string version = plugindir.Name.Split('_')[1];
            //                        string[] versionNumber = version.Split('.');
            //                        int[] number = new int[4];
            //                        if (versionNumber.Length > 2)
            //                        {
            //                            number[0] = int.Parse(versionNumber[0]);
            //                            number[1] = int.Parse(versionNumber[1]);
            //                            string revise = versionNumber[2].Replace("build", ".");
            //                            number[2] = int.Parse(revise.Split('.')[0]);
            //                            number[3] = int.Parse(revise.Split('.')[1]);
            //                            pl = CreatePlugin(plugindir, this, pluginName, number, true);
            //                            if (pl != null)
            //                            {
            //                                this.AddPlugin(pl);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            number[0] = int.Parse(versionNumber[0]);
            //                            number[1] = int.Parse(versionNumber[1]);
            //                            number[2] = int.MinValue;
            //                            number[3] = int.MinValue;
            //                            pl = CreatePlugin(plugindir, this, pluginName, number, true);
            //                            if (pl != null)
            //                            {
            //                                this.AddPlugin(pl);
            //                            }
            //                        }

            //                    }
            //                }
            //            }
            //            catch
            //            {
            //                sr.Close();
            //            }
            //            finally
            //            {
            //                sr.Close();
            //            }
            //        }
            //    }
            //}
            #endregion
        }

        private string plugindirKey = "plugindirKey";
        private string plugisKey = "plugisKey";
        private string nameKey = "nameKey";
        private string versionKey = "versionKey";
        private string loadedKey = "loadedKey";
        private string returnKey = "returnKey";

        private Plugin CreatePlugin(DirectoryInfo plugindir, Plugins plugis, string name, int[] version, bool loaded)
        {
            FileInfo[] files = plugindir.GetFiles("*.xml");
            bool isread = false;
            string loadVersion = null;
            bool iscreatAppDomain = true;
            if (files.Length <= 0)
            {
                return null;
            }
            else
            {
                ReadConfig read = new ReadConfig(files[0]);
                isread = read.IsLoadSystemCoreDir;
                loadVersion = read.LoadSystemCoreDirVersion;
                iscreatAppDomain = read.IsCreatNewDomain;
            }
            DirectoryInfo systemCore = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\lib");
            DirectoryInfo[] subdirectory = systemCore.GetDirectories();
            DirectoryInfo lastSystemCore = subdirectory[0];
            if (subdirectory.Length > 1)
            {                
                double systemVersion = 0.0;
                for (int i = 0; i < subdirectory.Length; i++)
                {
                    try
                    {
                        if (double.Parse(subdirectory[i].Name) > systemVersion)
                        {
                            lastSystemCore = subdirectory[i];
                        }
                    }
                    catch 
                    { }
                }
            }


            if (iscreatAppDomain)
            {
                string FriendlyName = name + "_" + version[0] + "." + version[1] + "." + version[2] + "build" + version[3];
                AppDomain appDoamin = AppDomain.CreateDomain(FriendlyName,
                    AppDomain.CurrentDomain.Evidence,
                    AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    lastSystemCore.FullName,
                    false);
                appDoamin.SetData(plugindirKey, plugindir);
                appDoamin.SetData(plugisKey, plugis);
                appDoamin.SetData(nameKey, name);
                appDoamin.SetData(versionKey, version);
                appDoamin.SetData(loadedKey, loaded);

                appDoamin.DoCallBack(() =>
                {
                    DirectoryInfo plugindirValue = AppDomain.CurrentDomain.GetData(plugindirKey) as DirectoryInfo;
                    Plugins plugisValue = AppDomain.CurrentDomain.GetData(plugisKey) as Plugins;
                    string nameValue = AppDomain.CurrentDomain.GetData(nameKey) as string;
                    int[] versionValue = AppDomain.CurrentDomain.GetData(versionKey) as int[];
                    bool loadedValue = (bool)AppDomain.CurrentDomain.GetData(loadedKey);
                    Plugin plugin = new Plugin(plugindirValue, plugisValue, nameValue, versionValue, loadedValue);
                    AppDomain.CurrentDomain.SetData(returnKey, plugin);
                });


                appDoamin.SetData(plugindirKey, null);
                appDoamin.SetData(plugisKey, null);
                appDoamin.SetData(nameKey, null);
                appDoamin.SetData(versionKey, null);
                appDoamin.SetData(loadedKey, null);

                return appDoamin.GetData(returnKey) as Plugin;
            }
            else
            {
                FileInfo[] dllFiles = lastSystemCore.GetFiles("*.dll");
                foreach (FileInfo item in dllFiles)
                {
                    try 
                    {
                        Assembly assembly = Assembly.LoadFrom(item.FullName);
                    }
                    catch { }
                }
                dllFiles = plugindir.GetFiles("*.dll");
                foreach (FileInfo item in dllFiles)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(item.FullName);
                    }
                    catch { }
                }
                //dllFiles = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase).GetFiles("*.dll");
                //foreach (FileInfo item in dllFiles)
                //{
                //    try
                //    {
                //        if (item.Name == "Lin.Core.dll")
                //        {
                //            Assembly assembly = Assembly.LoadFrom(item.FullName);
                //            break;
                //        }
                //    }
                //    catch { }
                //}
                Plugin plugin = new Plugin(plugindir, plugis, name, version, loaded);
                return plugin;
            }
        }
        /// <summary>
        /// 拿到当前的Plugins
        /// </summary>
        public static Plugins Current
        {
            get
            {
                Plugin plugin = Plugin.Current;
                if (plugin != null)
                {
                    return plugin.Plugins;
                }
                return null;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pluginpath"></param>
        public Plugins(DirectoryInfo pluginpath)
        {
            this.PluginPath = pluginpath;
            Init();
        }
        /// <summary>
        /// 获取所有插件
        /// </summary>
        /// <returns></returns>
        public IList<AddInToken> GetAddIns()
        {
            List<AddInToken> tokens = new List<AddInToken>();
            foreach (Plugin p in Pluginss)
            {
                if (p.DefaultLoad)
                {
                    tokens.AddRange(p.GetAddIns());
                }
            }
            return tokens;
        }
        /// <summary>
        /// 根据标识的类型获取所有插件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<AddInToken> GetAddIns(string type)
        {
            List<AddInToken> tokens = new List<AddInToken>();
            foreach (Plugin p in Pluginss)
            {
                if (p.DefaultLoad)
                {
                    tokens.AddRange(p.GetAddIns(type));
                }
            }
            return tokens;
        }
        /// <summary>
        /// 根据标识的特性查找所有特性类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<AttributeToken> GetAddIns(Type type)
        {
            List<AttributeToken> tokens = new List<AttributeToken>();
            foreach (Plugin p in Pluginss)
            {
                if (p.DefaultLoad)
                {
                    tokens.AddRange(p.GetAddIns(type));
                }
            }
            return tokens;
        }
        /// <summary>
        /// 根据插件标识的类型，以及表示了插件类的类类型来获取所有插件
        /// </summary>
        /// <param name="addintype"></param>
        /// <param name="classtype"></param>
        /// <returns></returns>
        public IList<AddInToken> GetAddIns(string addintype, Type classtype)
        {
            List<AddInToken> tokens = new List<AddInToken>();
            foreach (Plugin p in Pluginss)
            {
                if (p.DefaultLoad)
                {
                    tokens.AddRange(p.GetAddIns(addintype, classtype));
                }
            }
            return tokens;
        }
        /// <summary>
        /// 根据标识的类型，查找最新的插件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AddInToken GetLastAddIn(string type)
        {
            List<AddInToken> tokens = new List<AddInToken>();
            foreach (Plugin p in Pluginss)
            {
                if (p.DefaultLoad)
                {
                    AddInToken token = p.GetLastAddIn(type);
                    tokens.Add(token);
                }
            }
            AddInToken add = null;
            if (tokens != null && tokens.Count > 0)
            {
                add = tokens[0];
                for (int i = 1; i < tokens.Count; i++)
                {
                    if (add.Major < tokens[i].Major || (add.Major == tokens[i].Major && add.Minor < tokens[i].Minor))
                    {
                        add = tokens[i];
                    }
                }
            }
            return add;
        }
        /// <summary>
        /// 根据插件标识的类型，以及表示了插件类的类类型来获取最新的插件
        /// </summary>
        /// <param name="addintype"></param>
        /// <param name="classtype"></param>
        /// <returns></returns>
        public AddInToken GetLastAddIn(string addintype, Type classtype)
        {
            List<AddInToken> tokens = new List<AddInToken>();
            foreach (Plugin p in Pluginss)
            {
                if (p.DefaultLoad)
                {
                    AddInToken token = p.GetLastAddIn(addintype, classtype);
                    if (token != null)
                    {
                        tokens.Add(token);
                    }
                }
            }
            AddInToken add = null;
            if (tokens != null && tokens.Count > 0)
            {
                add = tokens[0];
                for (int i = 1; i < tokens.Count; i++)
                {
                    if (add.Major < tokens[i].Major || (add.Major == tokens[i].Major && add.Minor < tokens[i].Minor))
                    {
                        add = tokens[i];
                    }
                }
            }
            return add;
        }
        /// <summary>
        /// 根据名称获取单个插件操作对象
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        public Plugin GetPlugin(string plugin)
        {
            foreach (Plugin p in Pluginss)
            {
                if (p.Name == plugin)
                {
                    return p;
                }
            }
            return null;
        }




        private class sortClass : Comparer<MenuTab>
        {
            public override int Compare(MenuTab x, MenuTab y)
            {
                Math.Sign(x.Location - y.Location);
                return Math.Sign(x.Location - y.Location);
            }
        };
    }
}
