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
            CurrentDispatcher.BeginInvoke(new System.Action(() =>
                {
                    back(args);
                }), null);
            //}
        }
    }
}
