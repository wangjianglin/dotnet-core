using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ComponentAttribute
{
    /// <summary>
    /// 描述菜单按钮基类，包含菜单按钮的基本属性
    /// </summary>
    public class ButtonAttribute:System.Attribute
    {
        public ButtonAttribute()
        {
            DisableState = DisableState.Grey;
            NetState = null;
        }
        /// <summary>
        /// 按钮上显示的名称或标识的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 按钮上的背景图片地址
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 按钮所在组的名称
        /// </summary>
        public string TargetGroupName { get; set; }
        /// <summary>
        /// 按钮所在Tab标签的名称
        /// </summary>
        public string TargetTabName { get; set; }
        /// <summary>
        /// 按钮所触发的命令
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// 命令参数
        /// </summary>
        public object CommandParams { get; set; }
        /// <summary>
        /// 是否显示按钮所在组件的内容（true时直接显示按钮所在组件的内容，false时执行按钮所需执行的命令）
        /// </summary>
        public bool IsShowContent { get; set; }
        /// <summary>
        /// 在离线与在线的情况下按钮的显示状态
        /// </summary>
        public NetState? NetState { get; set; }
        /// <summary>
        /// 当按钮不能使用时的显示状态
        /// </summary>
        public DisableState DisableState { get; set; }
    }
}
