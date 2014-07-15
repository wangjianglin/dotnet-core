using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Web.Model
{
    /// <summary>
    /// 主要用以记录与服务器代码相关的版本信息
    /// </summary>
    public class Version
    {
        /// <summary>
        /// 服务代码对应版本管理工具的代码
        /// </summary>
        public int build { get; set; }
    }
}
