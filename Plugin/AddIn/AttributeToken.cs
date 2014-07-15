using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.AddIn
{
    [Serializable]
    public class AttributeToken//:MarshalByRefObject
    {
        public Type AttributeType { get; internal set; }
        public AppDomain AddInAppDomain { get; internal set; }
        public Attribute Attributes { get; internal set; }
    }
}
