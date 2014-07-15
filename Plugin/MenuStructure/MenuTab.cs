using Lin.Plugin.ComponentAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class MenuTab : List<MenuGroup>
    {
        public string TabName { get; set; }

        public double Location { get; set; }

        public string MenuShortcutKey { get; set; }

        public List<MenuTabAction> Actions { get; set; }
    }
}
