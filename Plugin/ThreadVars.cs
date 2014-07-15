using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lin.Plugin
{
    /// <summary>
    /// 返回当前线程储存的值
    /// </summary>
    public class ThreadUtilsIndex
    {
        private Dictionary<Thread, Dictionary<string, object>> objs = new Dictionary<Thread, Dictionary<string, object>>();

        private Thread thread;

        public ThreadUtilsIndex()
        {
            thread = new Thread(new ParameterizedThreadStart(obj =>
            {
                while (true)
                {
                    foreach (Thread item in objs.Keys)
                    {
                        if (!item.IsAlive)
                        {
                            objs.Remove(item);
                        }
                    }
                    Thread.Sleep(60000);
                }
            }));
            thread.Start();
        }

        public object this[string name]
        {
            get
            {
                Dictionary<string, object> values = objs[Thread.CurrentThread];
                if (values != null)
                {
                    return values[name];
                }
                return null;
            }
            set
            {
                if (objs.ContainsKey(Thread.CurrentThread))
                {
                    objs[Thread.CurrentThread].Add(name, value);
                }
                else
                {
                    objs.Add(Thread.CurrentThread, new Dictionary<string, object>());
                    objs[Thread.CurrentThread].Add(name, value);
                }
            }
        }
    }
    public static class ThreadVars
    {
        static ThreadVars()
        {
            if (Datas == null)
            {
                ThreadUtilsIndex tui = new ThreadUtilsIndex();
                Datas = tui;
            }
        }
        public static ThreadUtilsIndex Datas { get; private set; }
    }
}
