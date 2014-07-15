using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ComponentAttribute
{
    /// <summary>
    /// 选项陈列框特性类
    /// </summary>
    public class InRibbonGallery : ButtonAttribute
    {
        public int MaxItemsInRow { get; set; }

        public int MinItemsInRow { get; set; }

        public bool IsHorizontal { get; set; }

        public Dictionary<string, object> Items { get; set; }
    }
}
