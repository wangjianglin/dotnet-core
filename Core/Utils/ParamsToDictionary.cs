using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Utils
{
    public static class ParamsToDictionary
    {
        public static Dictionary<string, string> ToDictionary(string[] param)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (param != null || param.Length != 0)
            {
                for (int i = 0; i < param.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        dic.Add(param[i], param[i + 1]);
                    }
                }
                return dic;
            }
            else
            {
                return null;
            }
        }
    }
}
