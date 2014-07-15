using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ComponentAttribute
{
    /// <summary>
    /// 数字调整框
    /// </summary>
    public class Spinner : System.Attribute
    {
        public string FortmatString { get; set; }

        public int MaxValue { get; set; }

        public int MinValue { get; set; }

        public int Interval { get; set; }

    }
}
