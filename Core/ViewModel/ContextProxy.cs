using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Lin.Core.ViewModel2
{
    internal class ListenerEvent : MarshalByRefObject
    {
        private ContextProxy proxy = null;
        internal ListenerEvent(ContextProxy proxy)
        {
            this.proxy = proxy;
        }

        /// <summary>
        /// 监听Context对象中，属性改变事件
        /// </summary>
        /// <param name="propertyName"></param>
        public void Fire(string propertyName)
        {
            proxy.FirePropertyNameChanged(propertyName);
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
    [Serializable]
    public class ContextProxy : ViewModel, IContext
    {
        private bool isCrossDomain = false;

        private readonly string listenerKey = "Listener_Key_" + DateTime.Now.Ticks;
        internal ContextProxy()
        {
            AppDomain appdomain = GetAppDomain.GetSingleDomainAndCreate();
            appdomain.SetData(Thread.CurrentThread.ManagedThreadId + listenerKey, new ListenerEvent(this));
            appdomain.DoCallBack(() =>
            {
                ListenerEvent listener = AppDomain.CurrentDomain.GetData(Thread.CurrentThread.ManagedThreadId + listenerKey) as ListenerEvent;
                Context.GlobalImpl.AddListener(listener);
            });
            appdomain.SetData(listenerKey, null);
        }

        /// <summary>
        /// 监听Context对象中，属性改变事件
        /// </summary>
        /// <param name="propertyName"></param>
        internal void FirePropertyNameChanged(string propertyName)
        {
            this.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        private readonly string logLevelKey = "LogLevel_Key_" + DateTime.Now.Ticks;
        public Log.LogLevel LogLevel
        {
            get
            {
                if (!isCrossDomain)
                {
                    return Context.GlobalImpl.LogLevel;
                }
                else
                {
                    AppDomain appdomain = GetAppDomain.GetSingleDomainAndCreate();
                    appdomain.DoCallBack(() =>
                    {
                        AppDomain.CurrentDomain.SetData(Thread.CurrentThread.ManagedThreadId + logLevelKey, Context.GlobalImpl.LogLevel);
                        AppDomain.CurrentDomain.SetData(Thread.CurrentThread.ManagedThreadId + "", "--");
                    });

                    return (Log.LogLevel)appdomain.GetData(Thread.CurrentThread.ManagedThreadId + logLevelKey);
                }
            }
        }

        /// <summary>
        /// 是否连接网络成功(登录版)
        /// </summary>
        private readonly string isNetKey = "IsNet_Key_" + DateTime.Now.Ticks;
        public bool IsNet
        {
            get
            {
                if (!isCrossDomain)
                {
                    return Context.GlobalImpl.IsNet;
                }
                else
                {
                    AppDomain appdomain = GetAppDomain.GetSingleDomainAndCreate();
                    appdomain.DoCallBack(() =>
                    {
                        AppDomain.CurrentDomain.SetData(Thread.CurrentThread.ManagedThreadId + isNetKey, Context.GlobalImpl.IsNet);
                    });
                    return (bool)appdomain.GetData(Thread.CurrentThread.ManagedThreadId + isNetKey);
                }
            }
            set
            {
                if (!isCrossDomain)
                {
                    Context.GlobalImpl.IsNet = value;
                }
                else
                {
                    AppDomain appdomain = GetAppDomain.GetSingleDomainAndCreate();
                    appdomain.SetData(Thread.CurrentThread.ManagedThreadId + isNetKey, value);
                    appdomain.DoCallBack(() =>
                    {
                        Context.GlobalImpl.IsNet = (bool)AppDomain.CurrentDomain.GetData(Thread.CurrentThread.ManagedThreadId + isNetKey);
                    });
                    appdomain.SetData(Thread.CurrentThread.ManagedThreadId + isNetKey, null);
                }
            }
        }

        /// <summary>
        /// 全局依赖属性改变
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private readonly string propertyKey = "PropertyValue_Key_" + DateTime.Now.Ticks;
        private readonly string sKey = "S_Key_" + DateTime.Now.Ticks;
        private readonly string valueKey = "Value_Key_" + DateTime.Now.Ticks;
        public object this[string s]
        {
            get
            {
                if (!isCrossDomain)
                {
                    return null;//Context.GlobalImpl[s];
                }
                else
                {
                    AppDomain appdomain = GetAppDomain.GetSingleDomainAndCreate();
                    appdomain.SetData(Thread.CurrentThread.ManagedThreadId + sKey, s);
                    appdomain.DoCallBack(() =>
                    {
                        string obj = AppDomain.CurrentDomain.GetData(Thread.CurrentThread.ManagedThreadId + sKey).ToString();
                        //AppDomain.CurrentDomain.SetData(Thread.CurrentThread.ManagedThreadId + propertyKey, Context.GlobalImpl[obj]);
                    });
                    if (appdomain.GetData(Thread.CurrentThread.ManagedThreadId + propertyKey) != null)
                    {
                        return appdomain.GetData(Thread.CurrentThread.ManagedThreadId + propertyKey);
                    }
                }
                return null;
            }
            set
            {
                if (!isCrossDomain)
                {
                    //Context.GlobalImpl[s] = value;
                }
                else
                {
                    AppDomain appdomain = GetAppDomain.GetSingleDomainAndCreate();
                    appdomain.SetData(Thread.CurrentThread.ManagedThreadId + sKey, s);
                    appdomain.SetData(Thread.CurrentThread.ManagedThreadId + valueKey, value);
                    appdomain.DoCallBack(() =>
                    {
                        string objStr = "";
                        object sKeyObj = AppDomain.CurrentDomain.GetData(Thread.CurrentThread.ManagedThreadId + sKey);
                        if (sKeyObj != null)
                        {
                            objStr = sKeyObj.ToString();
                        }
                        object valueObj = AppDomain.CurrentDomain.GetData(Thread.CurrentThread.ManagedThreadId + valueKey);
                       // Context.GlobalImpl[objStr] = valueObj;
                    });
                    appdomain.SetData(Thread.CurrentThread.ManagedThreadId + sKey, null);
                    appdomain.SetData(Thread.CurrentThread.ManagedThreadId + valueKey, null);
                }
            }
        }
    }
}
