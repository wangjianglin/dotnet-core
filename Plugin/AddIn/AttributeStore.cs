using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lin.Plugin.AddIn
{
    public class AttributeStore
    {
        /// <summary>
        /// 存储该插件目录下面的所有程序集
        /// </summary>
        private List<Assembly> assemblys = new List<Assembly>();
        /// <summary>
        /// 加载插件目录下面所有的程序集
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="ext">需要加载的后缀名，默认为dll</param>
        public void Update(DirectoryInfo path, DirectoryInfo loadFilePath, string ext = "*.dll")
        {
            IEnumerable<FileInfo> files = path.GetFiles(ext);
            IEnumerable<FileInfo> Loadfiles = loadFilePath.GetFiles(ext);
            foreach (FileInfo file in files)
            {
                foreach (FileInfo item in Loadfiles)
                {
                    if (file.Name == item.Name)
                    {
                        try
                        {
                            //Assembly assembly = Assembly.LoadFile(file.FullName);
                            Assembly assembly = Assembly.LoadFrom(file.FullName);
                            assemblys.Add(assembly);
                            break;
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将该目录下的所有程序集为插件类型的类类型
        /// </summary>
        /// <param name="attributeType">需要查找的类型</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些特性</param>
        /// <returns></returns>
        public List<AttributeToken> FindAttributes(Type attributeType, bool inherit = false)
        {
            List<AttributeToken> list = new List<AttributeToken>();
            AttributeToken token=null;
            if (assemblys != null && assemblys.Count > 0)
            {
                object[] objs = null;
                foreach (Assembly assembly in assemblys)
                {
                    try
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            try
                            {
                                objs = type.GetCustomAttributes(attributeType, inherit);
                                if (objs != null && objs.Length > 0)
                                {
                                    foreach (object obj in objs)
                                    {
                                        if (obj as Attribute != null)
                                        {
                                            token = new AttributeToken() { AttributeType = type, AddInAppDomain = AppDomain.CurrentDomain, Attributes = obj as Attribute };
                                            list.Add(token);
                                        }
                                    }
                                }
                            }
                            catch (Exception) 
                            {

                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 根据所有的类型，查找到标识了特性类型的类类型
        /// </summary>
        /// <param name="type">查找的类型</param>
        /// <param name="attribute">特性类型</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些特性</param>
        /// <returns></returns>
        public List<Attribute> FindTypeAttributes(Type type, Type attribute, bool inherit = false)
        {
            if (type != null && attribute != null)
            {
                List<Attribute> attributes = new List<Attribute>();
                object[] objs = type.GetCustomAttributes(attribute, inherit);
                foreach (object obj in objs)
                {
                    if (obj.GetType() == attribute)
                    {
                        attributes.Add(obj as Attribute);
                    }
                }
                return attributes;
            }
            return null;
        }
    }
}
