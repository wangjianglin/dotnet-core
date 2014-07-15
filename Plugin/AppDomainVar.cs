using Lin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin
{
    public class AppDomainVar
    {
        private static Dictionary<string, object> dict = new Dictionary<string, object>();
        static AppDomainVar()
        {


            Vars = new IndexProperty<string, object>(name =>
            {
                if (dict.ContainsKey(name))
                {
                    return dict[name];
                }
                return null;
            }, (name, obj) =>
            {
                if (dict.ContainsKey(name))
                {
                    dict[name] = obj;
                }
                else
                {
                    dict.Add(name, obj);
                }
            });

            //Vars = new IndexProperty((name) =>
            //{
            //    if (dict.ContainsKey(name))
            //    {
            //        return dict[name];
            //    }
            //    return null;
            //}, (name, obj) => 
            //{
            //    if (dict.ContainsKey(name))
            //    {
            //        dict[name] = obj;
            //    }
            //    else
            //    {
            //        dict.Add(name, obj);
            //    }
            //});
        }
        public static IndexProperty<string, object> Vars { get; private set; }
    }
}
