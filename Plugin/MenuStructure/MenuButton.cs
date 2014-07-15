using Lin.Plugin.ComponentAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class MenuButton
    {
        public MenuButton()
        {
            DisableState = DisableState.Grey;
            NetState = null;
        }
        public string Name { get; set; }

        public string Icon { get; set; }

        public string TargetGroupName { get; set; }

        public string TargetTabName { get; set; }

        public string Command { get; set; }

        public object CommandParams { get; set; }

        public bool IsShowContent { get; set; }

        public NetState? NetState { get; set; }

        public DisableState DisableState { get; set; }

        public ViewToken ViewToken { get; set; }

        public ComponentStructure Component { get;internal set; }
    }
}
