using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lin.Plugin.AddIn
{
    public class AddInStore
    {
        private AttributeStore attributeStore;

        public AddInStore(DirectoryInfo path,DirectoryInfo loadPath)
        {
            attributeStore = new AttributeStore();
            attributeStore.Update(path, loadPath);
        }

        public AddInStore(AttributeStore attributeStore)
        {
            this.attributeStore = attributeStore;
        }

        /// <summary>
        /// 查找所有插件
        /// </summary>
        /// <returns></returns>
        public IList<AddInToken> FindAddIns()
        {
            List<AttributeToken> addIns = attributeStore.FindAttributes(typeof(AddIn));
            Attribute attributes;
            List<AddInToken> tokens = new List<AddInToken>();
            foreach (AttributeToken type in addIns)
            {
                attributes = type.Attributes;
                AddIn addin = (AddIn)attributes;
                if (addin != null)
                {
                    AddInToken token = new AddInToken(type.AttributeType);
                    CopyProperty(addin, token);
                    tokens.Add(token);
                }
            }
            tokens.Sort(new SortClass());
            return tokens;
        }

        private class SortClass : Comparer<AddInToken>
        {

            public override int Compare(AddInToken x, AddInToken y)
            {
                System.Math.Sign(x.Location - y.Location);
                return System.Math.Sign(x.Location - y.Location);
            }
        }

        /// <summary>
        /// 把src中的属性值copy到dest的目标属性中
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CopyProperty(object src, object dest, bool deep = false)
        {
            object tempObj = null;
            object newObj = null;
            if (dest == null)
            {
                return;
            }
            Type type = dest.GetType();
            PropertyInfo[] props = src.GetType().GetProperties();
            PropertyInfo tmp = null;
            foreach (PropertyInfo prop in props)
            {
                tmp = type.GetProperty(prop.Name);
                if (tmp == null)
                {
                    continue;
                }
                if (prop.CanRead && tmp.CanWrite)
                {
                    tempObj = prop.GetValue(src, null);
                    if (tempObj != null && deep)
                    {
                        if (tempObj.GetType().IsClass && !tempObj.GetType().IsPrimitive && tempObj.GetType() != typeof(string))
                        {
                            newObj = Activator.CreateInstance(tempObj.GetType());
                            CopyProperty(tempObj, newObj);
                            tempObj = newObj;
                        }
                    }
                    tmp.SetValue(dest, tempObj, null);
                }
            }
        }
    }
}
