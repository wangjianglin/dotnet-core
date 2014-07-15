using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExceute
    {
        void Exceute(params object[] obj);

        int Order { get; }
        //void Exceute(object obj);
    }
}
