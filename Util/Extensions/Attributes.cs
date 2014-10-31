using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Util.Extensions
{
    public static class Attributes
    {
        public static T GetCustomAttribute<T>(this Type type,bool inherit = false) where T : Attribute
        {
            Type customAttributeType = typeof(T);
            object[] objs = type.GetCustomAttributes(inherit);
            foreach (object obj in objs)
            {
                if (obj.GetType() == customAttributeType)
                {
                    return obj as T;
                }
            }
            return default(T);
        }
    }
}
