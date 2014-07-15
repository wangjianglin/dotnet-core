using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class MenuGroup : List<MenuButton>
    {
        public string Name { get; set; }

        public string TargetTabName { get; set; }

        //public bool IsLauncher { get; set; }

        public List<MenuGroupAtion> Actions { get; set; }
    }
}
