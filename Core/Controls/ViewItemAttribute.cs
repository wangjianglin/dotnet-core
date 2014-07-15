using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Controls
{
    public class ViewItemAttribute:Attribute
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string[] Params { get; set; }

        public string Description { get; set; }
    }
}
