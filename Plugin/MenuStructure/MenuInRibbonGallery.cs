using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class MenuInRibbonGallery : MenuButton
    {
        public int MaxItemsInRow { get; set; }

        public int MinItemsInRow { get; set; }

        public bool IsHorizontal { get; set; }

        public Dictionary<string, object> Items { get; set; }
    }
}
