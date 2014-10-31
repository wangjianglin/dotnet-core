using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Lin.Util;

namespace Lin.Util.Extensions
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 当Double类型的值为正无穷大或者负无穷大时，将其设置成英文
        /// </summary>
        /// <param name="d">当前转换对象的值</param>
        /// <returns></returns>
        public static string String(this double d)
        {
            string str = d.ToString();
            if (double.IsNegativeInfinity(d))
            {
                str = "-Inf";
            }
            else if (double.IsPositiveInfinity(d))
            {
                str = "Inf";
            }
            else if (double.IsNaN(d))
            {
                str = "NaN";
            }

            return str;
        }

        /// <summary>
        /// 当读取的文件中Double类型的值是正无穷大或者负无穷大的英文时，应将他转换过来
        /// </summary>
        /// <param name="str">当前读取的英文值</param>
        /// <param name="def">参数默认值</param>
        /// <returns></returns>
        public static double Double(this string str, double def = 0)
        {
            try
            {
                return System.Double.Parse(str);
            }
            catch(Exception ex)
            {
                //Lin.Core.Controls.TaskbarNotifierUtil.Show(new LinException(-0x2002010, ex));
            }

            return def;
        }

        /// <summary>
        /// copy 属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T CopyProperty<T>(this object src)
        {
            Type type = typeof(T);
            PropertyInfo[] props = src.GetType().GetProperties();
            T dest = (T)Activator.CreateInstance(type);
            PropertyInfo tmp = null;
            foreach (PropertyInfo prop in props)
            {
                tmp = type.GetProperty(prop.Name);
                if (tmp == null)
                {
                    continue;
                }
                if (prop.CanRead && tmp.CanWrite)
                {
                    tmp.SetValue(dest, prop.GetValue(src, null), null);
                }
            }
            return dest;
        }

        /// <summary>
        /// copy 属性
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static object CopyProperty(this object src)
        {
            Type type = src.GetType();
            PropertyInfo[] props = type.GetProperties();
            object dest = Activator.CreateInstance(type);
            foreach (PropertyInfo prop in props)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(dest, prop.GetValue(src, null), null);
                }
            }
            return dest;
        }

        /// <summary>
        /// 把src中的属性值copy到dest的目标属性中
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CopyProperty(this object src, object dest,bool deep = false)
        {
            object tempObj = null;
            object newObj = null;
            if (dest == null)
            {
                return;
            }
            Type type = dest.GetType();
            PropertyInfo[] props = src.GetType().GetProperties();
            PropertyInfo tmp = null;
            foreach (PropertyInfo prop in props)
            {
                tmp = type.GetProperty(prop.Name);
                if (tmp == null)
                {
                    continue;
                }
                if (prop.CanRead && tmp.CanWrite)
                {
                    tempObj = prop.GetValue(src, null);
                    if (tempObj != null && deep)
                    {
                        if (tempObj.GetType().IsClass && !tempObj.GetType().IsPrimitive && tempObj.GetType() != typeof(string))
                        {
                            newObj = Activator.CreateInstance(tempObj.GetType());
                            CopyProperty(tempObj, newObj);
                            tempObj = newObj;
                        }
                    }
                    tmp.SetValue(dest, tempObj, null);
                }
            }
        }
    }
}
