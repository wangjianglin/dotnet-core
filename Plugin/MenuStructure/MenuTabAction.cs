using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class MenuTabAction
    {
        /// <summary>
        /// 组件上Tab的名称
        /// </summary>
        public string TabName { get; set; }
        /// <summary>
        /// Tab所在的位置
        /// </summary>
        public double Location { get; set; }
        /// <summary>
        /// 组件的Tab的快捷菜单
        /// </summary>
        public string MenuShortKey { get; set; }
        /// <summary>
        /// Tab切换时所触发的命令
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// 命令参数
        /// </summary>
        public object CommandParams { get; set; }
        /// <summary>
        /// 行为所在的组件
        /// </summary>
        public ComponentStructure Component { get; internal set; }
    }
}
