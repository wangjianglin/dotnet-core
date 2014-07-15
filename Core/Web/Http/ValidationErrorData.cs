using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Web.Http
{
    /// <summary>
    /// 存储数据验证的错误信息
    /// </summary>
    public class ValidationErrorData
    {
        public string[] actionErrors { get; set; }
        public string[] actionMessages { get; set; }
        public IDictionary<string,string[]> errors { get; set; }
        public IDictionary<string, string[]> fieldErrors { get; set; } 
    }
}
