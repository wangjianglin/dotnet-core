using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ComponentAttribute
{
    /// <summary>
    /// 状态按钮描述类
    /// </summary>
    public class ToggleButton : ButtonAttribute
    {
        /// <summary>
        /// 一组状态按钮的名称(在这个组中，只有一个状态按钮会处于选择状态)
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 按钮是否为选中的状态
        /// </summary>
        public bool IsCheck { get; set; }
    }
}
