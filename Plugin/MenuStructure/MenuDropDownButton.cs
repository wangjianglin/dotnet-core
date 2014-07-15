using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class MenuDropDownButton : MenuButton
    {
        public Dictionary<string, string> DropImageButtons { get; set; }

        public int MaxItemsInRow { get; set; }

        public string DropImageButtonTag { get; set; }

        public List<DropMenuItem> DropItem { get; set; }
    }
}
