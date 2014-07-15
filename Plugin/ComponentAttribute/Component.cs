using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ComponentAttribute
{
    /// <summary>
    /// 组件描述类，描述一个单独的组件
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class Component : System.Attribute
    {
        /// <summary>
        /// 组件的名称
        /// </summary>
        public string Name { get; set; }       
        /// <summary>
        /// 组件所在的类
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 组件的描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 在联网与断网组件显示情况
        /// </summary>
        public NetState NetState { get; set; }
        /// <summary>
        /// 是否显示组件内容
        /// </summary>
        public bool IsVisual { get; set; }        
        /// <summary>
        /// 主版本号
        /// </summary>
        public uint Major { get; set; }
        /// <summary>
        /// 副版本号
        /// </summary>
        public uint Minor { get; set; }
    }
}
