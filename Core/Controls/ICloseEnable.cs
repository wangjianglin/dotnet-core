using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Controls
{
    /// <summary>
    /// 窗口关闭行为
    /// </summary>
    public enum WindowClosedBehavior
    {
        CANCEL, // 直接关闭不返回结果
        RESULT // 关闭窗口且能得到返回结果
    }

    public interface ICloseEnable
    {
        event EventHandler Closed;

        event EventHandler Canceled;

        object Result { get;}

        WindowClosedBehavior WindowClosedBehavior { get; }
    }
}
