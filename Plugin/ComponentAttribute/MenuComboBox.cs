using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ComponentAttribute
{
    /// <summary>
    /// 下拉选择款
    /// </summary>
    public class MenuComboBox : ButtonAttribute
    {
        /// <summary>
        /// 下拉的值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 值是否可以编辑
        /// </summary>
        public bool IsEdit { get; set; }
    }
}
