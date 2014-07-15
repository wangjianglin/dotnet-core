using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ComponentAttribute
{
    /// <summary>
    /// Group右下按钮行为描述类
    /// </summary>
    public class GroupAction : System.Attribute
    {
        /// <summary>
        /// Group的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Group所属Tab的名称
        /// </summary>
        public string TargetTabName { get; set; }
        /// <summary>
        /// 按钮所触发的命令
        /// </summary>
        public string LauncherCommand { get; set; }
        /// <summary>
        /// 命令参数
        /// </summary>
        public object CommandParams { get; set; }
    }
}
