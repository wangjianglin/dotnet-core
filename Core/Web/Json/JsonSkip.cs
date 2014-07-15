using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Web.Json
{
    /// <summary>
    /// 不对属性进行序列化，在JSON序列化与反序列化时，将跳过标记了JsonSkip特性的类或属性不做处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property)]
    public sealed class JsonSkip : Attribute
    {
    }
}
