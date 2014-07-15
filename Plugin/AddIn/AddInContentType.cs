using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.AddIn
{
    /// <summary>
    /// 插件的内容类型
    /// </summary>
    public enum AddInContentType
    {
        CONTROL,//控件
        EXCEUTE,//可执行的方法
        CONTENT //内容(可以为对象,但跨域对象须标记为可序列化的)
    }
}
