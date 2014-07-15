using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Lin.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        private static long preId = 0;
        private static object idLock = new object();
        /// <summary>
        /// 生成唯一的Id
        /// </summary>
        /// <returns></returns>
        public static long NextId()
        {
            lock (idLock)
            {
                long id = -1;
                while ((id = DateTime.Now.Ticks) == preId) ;
                preId = id;
                return id;
            }
        }
    }
}
