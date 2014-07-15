using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Util
{
    /// <summary>
    /// 
    /// </summary>
    public class LinExceptionWarningArgs : EventArgs
    {
        public LinExceptionWarningArgs(LinException warning)
        {
            this.Warning = warning;
        }
        public LinException Warning { get; private set; }
    }
}
