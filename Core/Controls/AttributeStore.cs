using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Lin.Core.Utils;

namespace Lin.Core.Controls
{
    public static class AttributeStore
    {
        private static List<Assembly> assemblys = new List<Assembly>();
        public static void Update(string path, string ext = "*.dll", bool isAppend = false)
        {
            if (!isAppend)
            {
                assemblys = new List<Assembly>();
            }
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                IEnumerable<FileInfo> files = dir.EnumerateFiles(ext);
                foreach (FileInfo file in files)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(file.FullName);
                        assemblys.Add(assembly);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Attribute> FindTypeAttributes(Type type, Type attribute, bool inherit = false)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static List<Type> FindAttributes(Type attributeType, bool inherit = false)
        {
            List<Type> list = new List<Type>();
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
                            objs = type.GetCustomAttributes(attributeType, inherit);
                            if (objs != null && objs.Length > 0)
                            {
                                list.Add(type);
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
            return list;
        }
    }
}
