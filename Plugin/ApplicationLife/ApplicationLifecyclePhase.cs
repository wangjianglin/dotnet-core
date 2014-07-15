using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ApplicationLife
{
    [Serializable]
    public enum ApplicationLifecyclePhase
    {
        ENTER_MAIN = 0,//进入Main方法
        ENTER_APP = 1,//开始初始化Application（实例化App）
        LEAVE_APP = 2,//初始化Application结束
        ENTER_ON_STARTUP = 3,//进入Application.OnStartUp
        LEAVE_ON_STARTUP = 4,//离开Application.OnStartUp
        ENTER_STARTUP_NEXT_INSTANCE = 5,//进入OnStartUpNextInstance
        LEAVE_STARTUP_NEXT_INSTANCE = 6,//离开OnStartUpNextInstance
        EXIT = 7 //Application退出
    }
}
