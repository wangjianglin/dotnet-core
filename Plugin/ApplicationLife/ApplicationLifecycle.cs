using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ApplicationLife
{
    [AttributeUsage(AttributeTargets.Method)]

    public class ApplicationLifecycle:Attribute
    {
        public ApplicationLifecycle(ApplicationLifecyclePhase Phase, int Order = int.MaxValue)
        {
            this.Phase = Phase;
            this.Order = Order;
        }
        public ApplicationLifecyclePhase Phase { get; private set; }
        public int Order { get; private set; }
    }
}
