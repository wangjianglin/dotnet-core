using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class MenuSplitButton : MenuButton
    {
        public Dictionary<string, string> DropImageButton { get; set; }

        public int MaxItemsInRow { get; set; }

        public DropMenuItem DropItem { get; set; }
    }
}
