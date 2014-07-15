using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ComponentAttribute
{
    /// <summary>
    /// 网络状态
    /// </summary>
    public enum NetState
    {
        OnlineAndOffline, //联网和离线时
        OnlyOnline,      //仅联网时
        OnlyOneOffline   //仅离线时
    }
}
