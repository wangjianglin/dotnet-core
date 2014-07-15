using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class MenuSpinner : MenuButton
    {
        public string FortmatString { get; set; }

        public int MaxValue { get; set; }

        public int MinValue { get; set; }

        public int Interval { get; set; }
    }
}
