using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Lin.Plugin.AddIn
{
    public class XmlStore
    {
        /// <summary>
        /// 保存当前插件目录下的所有插件
        /// </summary>
        private List<AddInToken> FindPlugin;
        /// <summary>
        /// 保存当前插件目录下面的所有程序集
        /// </summary>
        private List<Assembly> assemblys;
        /// <summary>
        /// 获取所有的插件信息并返回
        /// </summary>
        /// <param name="path">插件目录</param>
        /// <returns></returns>
        public List<AddInToken> GetPlugins(DirectoryInfo path)
        {
            FindPlugin = new List<AddInToken>();
            this.Update(path);
            FileInfo[] files = path.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Extension == ".xml")
                {
                    return ReadXml(files[i]);
                }
            }
            return null;
        }
        /// <summary>
        /// 获取当前插件目录下面的所有程序集
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        private List<Assembly> Update(DirectoryInfo path, string ext = "*.dll")
        {
            assemblys = new List<Assembly>();
            IEnumerable<FileInfo> files = path.GetFiles(ext);
            foreach (FileInfo file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file.FullName);
                    assemblys.Add(assembly);
                }
                catch
                {

                }
            }
            return assemblys;
        }
        /// <summary>
        /// 通过插件目录下的XML文件信息得到该目录下面的所有插件信息
        /// </summary>
        /// <param name="XmlPath"></param>
        /// <returns></returns>
        private List<AddInToken> ReadXml(FileInfo XmlPath)
        {
            XDocument doc = XDocument.Load(XmlPath.FullName);
            IEnumerable<XElement> addIns = doc.Descendants("AddIn");
            foreach (XElement addin in addIns)
            {
                AddInToken token = new AddInToken();
                IEnumerable<XNode> nodes = addin.Nodes();
                List<XElement> addinElement = new List<XElement>();
                foreach (XNode node in nodes)
                {
                    XElement tmp = node as XElement;
                    if (tmp != null)
                    {
                        if (tmp.Name.ToString() == "AssemblyName")
                        {
                            foreach (Assembly assembly in assemblys)
                            {
                                if (assembly.GetName().Name == tmp.Value)
                                {
                                    token.AddInAssembly = assembly;
                                }
                            }
                        }
                        if (tmp.Name.ToString() == "ClassFullName")
                        {
                            token.ClassFullName = tmp.Value;
                        }
                        if (tmp.Name.ToString() == "Type")
                        {
                            token.Type = tmp.Value;
                        }
                        if (tmp.Name.ToString() == "Description")
                        {
                            token.Description = tmp.Value;
                        }
                        if (tmp.Name.ToString() == "Params")
                        {
                            List<XNode> paramss = tmp.Nodes().ToList();
                            if (paramss.Count > 0)
                            {
                                string[] tmpparams = new string[paramss.Count];
                                for (int i = 0; i < paramss.Count; i++)
                                {
                                    XElement paramElement = paramss[i] as XElement;
                                    if (paramElement != null)
                                    {
                                        tmpparams[i] = paramElement.Value;
                                    }
                                }
                                token.Params = tmpparams;
                            }

                        }
                        if (tmp.Name.ToString() == "Publisher")
                        {
                            token.Publisher = tmp.Value;
                        }
                        if (tmp.Name.ToString() == "Location")
                        {
                            try 
                            {
                                token.Location = double.Parse(tmp.Value);
                            }
                            catch 
                            {
                                token.Location = 0.0;
                            }
                        }
                        if (tmp.Name.ToString() == "Version")
                        {
                            token.Version = tmp.Value;
                        }
                        if (tmp.Name.ToString() == "Major")
                        {
                            try
                            {
                                token.Major = uint.Parse(tmp.Value);
                            }
                            catch
                            {
                                token.Major = 1;
                            }
                        }
                        if (tmp.Name.ToString() == "Minor")
                        {
                            try 
                            {
                                token.Minor = uint.Parse(tmp.Value);
                            }
                            catch 
                            {
                                token.Minor = 0;
                            }
                        }
                    }
                }
                FindPlugin.Add(token);
            }
            FindPlugin.Sort(new SortClass());
            return FindPlugin;
        }

        private class SortClass : Comparer<AddInToken>
        {

            public override int Compare(AddInToken x, AddInToken y)
            {
                System.Math.Sign(x.Location - y.Location);
                return System.Math.Sign(x.Location - y.Location);
            }
        }
    }
}
