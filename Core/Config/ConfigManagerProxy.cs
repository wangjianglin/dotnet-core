using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Lin.Core.Config
{
    /// <summary>
    /// 监听属性改变事件
    /// </summary>
    internal class ListenerEvent : MarshalByRefObject
    {
        private ConfigManagerProxy proxy = null;
        internal ListenerEvent(ConfigManagerProxy proxy)
        {
            this.proxy = proxy;
        }

        /// <summary>
        /// 监听ConfigManager对象中，属性改变事件
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
    public class ConfigManagerProxy : IConfigManager
    {
        private string section = "";
        private readonly string listenerKey = "Listener_Key_" + DateTime.Now.Ticks;
        internal ConfigManagerProxy(string section)
        {
            this.section = section;
            AppDomain appdomain = GetAppDomain.GetSingleDomainAndCreate();
            appdomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + listenerKey, new ListenerEvent(this));
            appdomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + sectionKey, section);
            appdomain.DoCallBack(() =>
            {
                ListenerEvent listener = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + listenerKey) as ListenerEvent;
                ConfigManager.ProxyGetConfigManager(AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + sectionKey).ToString()).AddListener(listener);
            });
            appdomain.SetData(listenerKey, null);
        }

        /// <summary>
        /// Section数组
        /// </summary>
        private readonly string sectionKey = "Section_Key_" + DateTime.Now.Ticks;
        public string[] Section
        {
            get
            {
                AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
                appDomain.DoCallBack(() =>
                {
                    string[] sectionObj = ConfigManager.ProxyGetConfigManager(section).Section;
                    AppDomain.CurrentDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + sectionKey, sectionObj);
                });
                if (appDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + sectionKey) != null)
                {
                    return appDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + sectionKey) as string[];
                }
                return null;
            }
        }

        /// <summary>
        /// Section下属性的值
        /// </summary>
        private readonly string ValuesKey = "Value_Key_" + DateTime.Now.Ticks;
        public string[] Values
        {
            get
            {
                AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
                appDomain.DoCallBack(() =>
                {
                    string[] valuesObj = ConfigManager.ProxyGetConfigManager(section).Values;
                    AppDomain.CurrentDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + ValuesKey, valuesObj);
                });
                if (appDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + ValuesKey) != null)
                {
                    return appDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + ValuesKey) as string[];
                }
                return null;
            }
        }

        /// <summary>
        /// Section下的名称数组
        /// </summary>
        private readonly string namesKey = "Names_Key_" + DateTime.Now.Ticks;
        public string[] Names
        {
            get
            {
                AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
                appDomain.DoCallBack(() =>
                {
                    string[] namesObj = ConfigManager.ProxyGetConfigManager(section).Names;
                    AppDomain.CurrentDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + namesKey, namesObj);
                });
                if (appDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + namesKey) != null)
                {
                    return appDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + namesKey) as string[];
                }
                return null;
            }
        }

        /// <summary>
        /// 指定属性名称name设置或获取值
        /// </summary>
        private readonly string nameKey = "Name_Key_" + DateTime.Now.Ticks;
        private readonly string propertyValueKey = "PropertyValue_Key_" + DateTime.Now.Ticks;
        private readonly string valueKey = "Value_Key_" + DateTime.Now.Ticks;
        public string this[string name]
        {
            get
            {
                AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
                appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + nameKey, name);
                appDomain.DoCallBack(() =>
                {
                    string nameKeyObj = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + nameKey).ToString();
                    string propertyValueObj = ConfigManager.ProxyGetConfigManager(section)[nameKeyObj];
                    AppDomain.CurrentDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + propertyValueKey, propertyValueObj);
                });
                if (appDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + propertyValueKey) != null)
                {
                    return appDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + propertyValueKey).ToString();
                }
                return null;
            }
            set
            {
                AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
                appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + nameKey, name);
                appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + valueKey, value);
                appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + sectionKey, section);
                appDomain.DoCallBack(() =>
                {
                    string nameKeyObjValue = "";
                    string valueObjValue = "";
                    object nameObj = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + nameKey);
                    object valueObj = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + valueKey);
                    if (nameObj != null)
                    {
                        nameKeyObjValue = nameObj.ToString();
                    }
                    if (valueObj != null)
                    {
                        valueObjValue = valueObj.ToString();
                    }
                    ConfigManager.ProxyGetConfigManager(AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + sectionKey).ToString())[nameKeyObjValue] = valueObjValue;
                });
            }
        }

        /// <summary>
        /// 监听属性改变事件时，发生
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        internal void FirePropertyNameChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
