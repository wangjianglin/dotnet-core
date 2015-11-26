using Lin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace Lin.Core
{
    public class Thread
    {
        static Thread()
        {
            InitVars();
        }
        public Thread(SynchronizationContext context)
        {
            this.Context = context;
        }
        private SynchronizationContext Context { get; set; }

        /// <summary>
        /// 主线程回到Context线程
        /// </summary>
        /// <param name="back"></param>
        /// <param name="args"></param>
        public void Post(Action<object> back, object args = null)
        {
            if (Context != null)
            {
                Context.Post(obj =>
                {
                    back(args);
                }, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="back"></param>
        /// <param name="args"></param>
        public void Send(Action<object> back, object args = null)
        {
            if (Context != null)
            {
                Context.Send(obj =>
                {
                    back(args);
                }, null);
            }
        }


        /// <summary>
        /// 判断当前线程是否是UI线程
        /// </summary>
        /// <returns>true表示当前线程是UI线程，false表示不是UI线程</returns>
        public static bool IsUIThread()
        {
            //Dispatcher.CurrentDispatcher.
            return Dispatcher.CurrentDispatcher == System.Windows.Application.Current.Dispatcher;
        }

        /// <summary>
        /// 启动后台线程
        /// </summary>
        /// <param name="back"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static System.Threading.Thread BackThread(Action<object> back, object args = null)
        {
            System.Threading.Thread thread = new System.Threading.Thread(state =>
            {
                back(args);
            });
            thread.IsBackground = true;
            thread.Start();
            return thread;
        }

        public static Dispatcher _CurrentDispatcher = null;
        public static Dispatcher CurrentDispatcher
        {
            get 
            {
                if (_CurrentDispatcher != null)
                {
                    return _CurrentDispatcher;
                }
                else
                {
                    if (System.Windows.Application.Current != null)
                    {
                        return System.Windows.Application.Current.Dispatcher;
                    }
                    return null;
                }
            }
            set 
            {
                _CurrentDispatcher = value;
            }
        }
        /// <summary>
        /// 启动界面线程
        /// </summary>
        /// <param name="back"></param>
        /// <param name="args"></param>
        public static void UIThread(Action<object> back, object args = null)
        {
            //if (IsUIThread())
            //{
            //    back(args);
            //}
            //else
            //{
            //Dispatcher.CurrentDispatcher
            if (CurrentDispatcher != null) { 
                CurrentDispatcher.BeginInvoke(new System.Action(() =>
                    {
                        back(args);
                    }), null);
            }
        }


        public static IndexProperty<string,object> Vars { get; private set; }

        private static void InitVars()
        {
            Vars = new IndexProperty<string, object>(name =>
            {
                return varsImpl[name];
            }, (name, value) =>
            {
                varsImpl[name] = value;
            });
        }

        private static ThreadVarsImpl varsImpl = new ThreadVarsImpl();

        private class ThreadVarsImpl
        {
            private Dictionary<System.Threading.Thread, Dictionary<string, object>> objs = new Dictionary<System.Threading.Thread, Dictionary<string, object>>();

            private System.Threading.Thread thread;

            public ThreadVarsImpl()
            {
                thread = new System.Threading.Thread(new ParameterizedThreadStart(obj =>
                {
                    while (true)
                    {
                        foreach (System.Threading.Thread item in objs.Keys)
                        {
                            if (!item.IsAlive)
                            {
                                objs.Remove(item);
                            }
                        }
                        System.Threading.Thread.Sleep(60000);
                    }
                }));
                thread.Start();
            }

            public object this[string name]
            {
                get
                {
                    Dictionary<string, object> values = objs[System.Threading.Thread.CurrentThread];
                    if (values != null)
                    {
                        return values[name];
                    }
                    return null;
                }
                set
                {
                    if (objs.ContainsKey(System.Threading.Thread.CurrentThread))
                    {
                        objs[System.Threading.Thread.CurrentThread].Add(name, value);
                    }
                    else
                    {
                        objs.Add(System.Threading.Thread.CurrentThread, new Dictionary<string, object>());
                        objs[System.Threading.Thread.CurrentThread].Add(name, value);
                    }
                }
            }
        }
    }
}
