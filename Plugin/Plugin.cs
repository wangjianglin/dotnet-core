using Lin.Plugin.AddIn;
using Lin.Plugin.MenuStructure;
using Lin.Util.Assemblys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Lin.Plugin
{
    //[Serializable]
    /// <summary>
    /// 当个插件
    /// </summary>
    public class Plugin : MarshalByRefObject
    {
        /// <summary>
        /// 当个插件目录
        /// </summary>
        private DirectoryInfo PluginDir;
        /// <summary>
        /// 没有合并前的插件目录
        /// </summary>
        //private DirectoryInfo OriginalPluginDir;

        /// <summary>
        ///插件目录下的所有插件集合 
        /// </summary>
        private IList<AddInToken> _AddInTokens;
        private IList<AddInToken> AddInTokens
        {
            get
            {
                GetAddInsImpl();
                return _AddInTokens;
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 版本信息
        /// </summary>
        public int[] Version;
        /// <summary>
        /// 表示是否加载该版本
        /// </summary>
        public bool DefaultLoad { get; private set; }

        private static Dictionary<string, Plugin> appDomainToPlugin = new Dictionary<string, Plugin>();
        /// <summary>
        /// 当前操作对象
        /// </summary>
        public static Plugin Current
        {
            get
            {

                System.Diagnostics.StackTrace s = new System.Diagnostics.StackTrace();
                string fullName = s.GetFrame(0).GetMethod().DeclaringType.Assembly.CodeBase;
                string dirName = null;
                for (int n = 0; ; n++)
                {
                    fullName = s.GetFrame(n).GetMethod().DeclaringType.Assembly.CodeBase;
                    int index = fullName.IndexOf("///");
                    string filename = fullName.Substring(index + 3);
                    dirName = new FileInfo(filename).Directory.FullName;
                    if (appDomainToPlugin.ContainsKey(dirName))
                    {
                        return appDomainToPlugin[dirName];
                    }
                }
            }
        }

        /// <summary>
        /// 所属的Plugins
        /// </summary>
        public Plugins Plugins { get; internal set; }


        private AddInStore addinstore = null;
        private AssemblyStore attributeStore = null;
        private ComponentStore componentStore = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="plugindir"></param>
        /// <param name="plugis"></param>
        /// <param name="loaded"></param>
        public Plugin(DirectoryInfo plugindir, Plugins plugis, string name, int[] version, bool loaded)
        {
            this.PluginDir = plugindir;
            this.Plugins = plugis;
            this.Name = name;
            this.Version = version;
            this.DefaultLoad = loaded;
            appDomainToPlugin.Add(plugindir.FullName, this);
            attributeStore = new AssemblyStore();
            attributeStore.Update(plugindir);
            addinstore = new AddInStore(attributeStore);
        }

        /// <summary>
        /// 返回组件数据结构
        /// </summary>
        public List<ComponentStructure> ComponentStructures
        {
            get
            {
                if (attributeStore != null)
                {
                    if (componentStore != null)
                    {
                        return componentStore.FindComponent();
                    }
                    else
                    {
                        componentStore = new ComponentStore(attributeStore);
                        return componentStore.FindComponent();
                    }
                }
                return new List<ComponentStructure>();
            }
        }

        /// <summary>
        /// 返回早先视图组件数据结构
        /// </summary>
        public List<ViewToken> ViewTokens
        {
            get 
            {
                if (attributeStore != null)
                {
                    if (componentStore != null)
                    {
                        return componentStore.FindView();
                    }
                    else
                    {
                        componentStore = new ComponentStore(attributeStore);
                        return componentStore.FindView();
                    }
                }
                return new List<ViewToken>();
            }
        }

        /// <summary>
        /// 获取插件目录下的所有插件
        /// </summary>
        /// <returns></returns>
        public IList<AddInToken> GetAddIns()
        {
            return new List<AddInToken>(AddInTokens);
        }        
        /// <summary>
        /// 使用XML或者attribute方式获取所有插件
        /// </summary>
        private void GetAddInsImpl()
        {
            bool loadModel = true;
            FileInfo[] files = PluginDir.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Extension.ToLower() == ".xml")
                {
                    loadModel = this.PluginLoadModel(files[i]);
                }
            }
            if (loadModel)
            {                
                this._AddInTokens = addinstore.FindAddIns();
                if (this._AddInTokens != null && this._AddInTokens.Count > 0)
                {
                    for (int i = 0; i < this._AddInTokens.Count; i++)
                    {
                        this._AddInTokens[i].AddInAppDomain = AppDomain.CurrentDomain;
                    }
                }
            }
            else
            {
                XmlStore xmlstore = new XmlStore();
                this._AddInTokens = xmlstore.GetPlugins(PluginDir);
                if (this._AddInTokens != null && this._AddInTokens.Count > 0)
                {
                    for (int i = 0; i < this.AddInTokens.Count; i++)
                    {
                        this._AddInTokens[i].AddInAppDomain = AppDomain.CurrentDomain;
                    }
                }
            }         
        }
        /// <summary>
        /// 根据插件表示的插件类型查找所有插件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<AddInToken> GetAddIns(string type)
        {
            List<AddInToken> typeTokens = new List<AddInToken>();
            for (int i = 0; i < AddInTokens.Count; i++)
            {
                if (AddInTokens[i].Type == type)
                {
                    typeTokens.Add(AddInTokens[i]);
                }
            }
            return typeTokens;
        }

        /// <summary>
        /// 根据特性类型查找插件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        //public List<AttributeToken> GetAddIns(Type type)
        //{
        //    //Lin.Plugin.AddIn.AttributeStore store = new AttributeStore();
        //    //store.Update(this.PluginDir);
        //    //store.Update(new DirectoryInfo(@"D:\ECM\ECMTest\bin\Debug\lib\1.0"));
        //    List<AttributeToken> attribute = this.attributeStore.FindAttributes(type);
        //    return attribute;
        //}

        /// <summary>
        /// 根据插件标识的类型，以及表示了插件类的类类型来获取插件
        /// </summary>
        /// <param name="addintype"></param>
        /// <param name="classtype"></param>
        /// <returns></returns>
        public List<AddInToken> GetAddIns(string addintype, Type classtype)
        {
            List<AddInToken> typeTokens = new List<AddInToken>();

            for (int i = 0; i < AddInTokens.Count; i++)
            {
                if (AddInTokens[i].Type == addintype && classtype.IsInstanceOfType(Activator.CreateInstance((Type)AppDomainVar.Vars[AddInTokens[i].AddInTypeName])))
                {
                    typeTokens.Add(AddInTokens[i]);
                }
            }
            return typeTokens;
        }
        /// <summary>
        /// 根据插件标识的类型，查找到最新的插件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AddInToken GetLastAddIn(string type)
        {
            IList<AddInToken> tokens = this.GetAddIns(type);
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
        /// 根据插件标识的类型，以及表示了插件类的类类型来查找最新插件
        /// </summary>
        /// <param name="addintype"></param>
        /// <param name="classtype"></param>
        /// <returns></returns>
        public AddInToken GetLastAddIn(string addintype, Type classtype)
        {
            IList<AddInToken> tokens = this.GetAddIns(addintype, classtype);
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
        /// 根据XML得到插件的加载方式
        /// </summary>
        /// <param name="xmlfile">XML文件</param>
        /// <returns></returns>
        private bool PluginLoadModel(FileInfo xmlfile)
        {
            XDocument xml = XDocument.Load(xmlfile.FullName);
            IEnumerable<XElement> elements = xml.Descendants("Auto");
            foreach (XElement element in elements)
            {
                string isauto = element.Value;
                if (isauto.ToLower().Trim() == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
