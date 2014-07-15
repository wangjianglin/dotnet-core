using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Web.Json
{
    /// <summary>
    /// 确定在序列化与反序列化时采用的格式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonFormat:Attribute
    {
        public string Format { get; set; }
    }
}
