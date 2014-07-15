using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Log
{

    /// <summary>
    /// 日记级别  
    /// </summary>
    [Serializable]
    public enum LogLevel
    {
        DEBUG = 0,
        INFO = 1,
        WARNING = 2,
        ERROR = 3
    }
}
