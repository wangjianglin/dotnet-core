using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Lin.Core.Cache;
using Lin.Core.Log;

namespace Lin.Core.ViewModel2
{
    public interface IContext:INotifyPropertyChanged
    {
        LogLevel LogLevel { get; }
        bool IsNet { set; get; }
        //object this[string s] { set; get; }
    }
}
