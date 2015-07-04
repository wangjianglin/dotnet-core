using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Comm.Http
{
    /// <summary>
    /// 数据包的版本信息
    /// 
    /// 
    /// 版本控制比较普遍的 3 种命名格式 :  一、 GNU 风格的版本号命名格式 :
    ///  主版本号 . 子版本号 [. 修正版本号 [. 编译版本号 ]]
    ///  英文对照 : Major_Version_Number.Minor_Version_Number[.Revision_Number[.Build_Number]]
    ///  示例 : 1.2.1, 2.0, 5.0.0 build-13124
    ///  二、 Windows 风格的版本号命名格式 :
    ///  主版本号 . 子版本号 [ 修正版本号 [. 编译版本号 ]]
    ///  英文对照 : Major_Version_Number.Minor_Version_Number[Revision_Number[.Build_Number]]
    ///  示例: 1.21, 2.0
    ///  三、.Net Framework 风格的版本号命名格式:
    ///  主版本号.子版本号[.编译版本号[.修正版本号]]
    ///  英文对照: Major_Version_Number.Minor_Version_Number[.Build_Number[.Revision_Number]]
    ///  版本号由二至四个部分组成：主版本号、次版本号、内部版本号和修订号。主版本号和次版本号是必选的；内部版本号和修订号是可选的，但是如果定义了修订号部分，则内部版本号就是必选的。所有定义的部分都必须是大于或等于 0 的整数。
    ///
    /// 数据包版本号只包括：主版本号和次版本号
    /// 
    /// 原则上，只要接口发生变化，次版本号就1，也必须加1及以上
    ///     系统有重大升级时，主版本号加1
    ///     
    /// 如果Major和Minor都为0则表示无版本号，也就是说此数据包在任何情况都可以被处理
    /// </summary>
    public class Version
    {
        /// <summary>
        /// 主版本号
        /// </summary>
        public uint Major { get; set; }
        /// <summary>
        /// 次版本号
        /// </summary>
        public uint Minor { get; set; }
    }
}
