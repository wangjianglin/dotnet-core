using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin
{
    public class ProxyExceute:MarshalByRefObject,IExceute
    {
        private string key;

        public ProxyExceute(string key)
        {
            this.key = key;
        }

        public void Exceute(object[] obj)
        {
            IExceute e = AppDomainVar.Vars[key] as IExceute;
            e.Exceute(obj);
        }


        public int Order
        {
            get { return 0; }
        }
    }
}
