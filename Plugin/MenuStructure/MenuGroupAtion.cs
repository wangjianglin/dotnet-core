using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class MenuGroupAtion
    {
        public string Name { get; set; }

        public string TargetTabName { get; set; }

        public string LauncherCommand { get; set; }

        public object CommandParams { get; set; }

        public ComponentStructure Component { get; internal set; }
    }
}
