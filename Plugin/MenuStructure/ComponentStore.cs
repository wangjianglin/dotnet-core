using Lin.Plugin.AddIn;
using Lin.Plugin.ComponentAttribute;
using Plugin.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    /// <summary>
    /// 寻找组件类
    /// </summary>
    internal class ComponentStore
    {
        private AttributeStore attributeStore;

        internal ComponentStore(DirectoryInfo path, DirectoryInfo loadPath)
        {
            attributeStore = new AttributeStore();
            attributeStore.Update(path, loadPath);
        }

        internal ComponentStore(AttributeStore attributeStore)
        {
            this.attributeStore = attributeStore;
        }

        /// <summary>
        /// 找到所有组件，并将它们转换成组件数据结构进行返回
        /// </summary>
        /// <returns>所有组件数据结构集合</returns>
        internal List<ComponentStructure> FindComponent()
        {
            List<ComponentStructure> componentStructrues = new List<ComponentStructure>();
            List<AttributeToken> components = attributeStore.FindAttributes(typeof(Component));
            ComponentStructure componentStructure = null;
            foreach (AttributeToken type in components)
            {
                Component component = (Component)type.Attributes;
                if (component != null)
                {
                    componentStructure = new ComponentStructure(type.AttributeType);
                    CopyProperty(component, componentStructure);
                    componentStructrues.Add(componentStructure);
                }
            }
            return componentStructrues;
        }

        /// <summary>
        /// 返回早先业务组件
        /// </summary>
        /// <returns></returns>
        internal List<ViewToken> FindView()
        {
            List<AttributeToken> views = attributeStore.FindAttributes(typeof(ViewAttribute));
            ViewToken token = null;
            List<ViewToken> tokens = new List<ViewToken>();
            foreach (AttributeToken type in views)
            {
                ViewAttribute view = (ViewAttribute)type.Attributes;
                if (view != null)
                {
                    token = new ViewToken(type.AttributeType);
                    CopyProperty(view, token);
                    tokens.Add(token);
                }
            }
            return tokens;
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
