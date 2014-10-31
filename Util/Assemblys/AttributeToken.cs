using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Util.Assemblys
{
    [Serializable]
    public class AttributeToken<T> where T:Attribute//,new()//:MarshalByRefObject
    {
        public Type OwnerType { get; internal set; }
        public AppDomain AddInAppDomain { get; internal set; }
        public T Attribute { get; internal set; }
    }
}
