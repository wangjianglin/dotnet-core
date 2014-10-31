using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Util
{
    /// <summary>
    /// 实现对弱引用的操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakReferenceCollection<T>
    {
        private IList<WeakReference> wrs = new List<WeakReference>();
        public void Add(T obj)
        {
            wrs.Add(new WeakReference(obj));
        }

        public void Remove(T obj)
        {
            object tmp = null;
            foreach (WeakReference wr in wrs)
            {
                tmp = wr.Target;
                if (tmp != null)
                {
                    T t = (T)tmp;
                    if (t.GetHashCode() == obj.GetHashCode())//如果是同一对象
                    {
                        wrs.Remove(wr);
                        return;
                    }
                }
                else
                {
                    wrs.Remove(wr);
                }
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="action"></param>
        public void Action(Action<T> action)
        {
            object tmp = null;
            List<WeakReference> removes = new List<WeakReference>();
            foreach (WeakReference wr in wrs)
            {
                tmp = wr.Target;
                if (tmp != null)
                {
                    action((T)tmp);
                }
                else
                {
                    removes.Add(wr);
                }
            }
            foreach (WeakReference wr in removes)
            {
                wrs.Remove(wr);
            }
        }
    }
}
