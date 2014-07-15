using Lin.Plugin.ComponentAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    /// <summary>
    /// 组件数据结构
    /// </summary>
    public class ComponentStructure
    {
        public ComponentStructure(Type ComponentType)
        {
            this.ComponentType = ComponentType;
        }
        /// <summary>
        /// 组件的名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type ComponentType { get; internal set; }

        private object _Content;
        public object Content
        {
            get 
            {
                if (ComponentType != null)
                {
                    if (_Content == null)
                    {
                        _Content = Activator.CreateInstance(ComponentType);
                    }
                    return _Content;
                }
                else
                {
                    return null;
                }
            }
        }

        public string Type { get; set; }

        public string Description { get; set; }

        public NetState? NetState { get; set; }

        public bool IsVisual { get; set; }

        public uint Major { get; set; }

        public uint Minor { get; set; }
    }
}
