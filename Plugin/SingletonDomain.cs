using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;

namespace Lin.Plugin
{
    /// <summary>
    /// 应用程序共享数据类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonDomain<T> : MarshalByRefObject where T : new()
    {
        private static readonly string AppDomainName = "SingletonAppDomain";
        private static T _instance;

        private static AppDomain GetAppDomain(string friendlyName)
        {
            IntPtr enumHandle = IntPtr.Zero;
            mscoree.CorRuntimeHost host = new mscoree.CorRuntimeHost();
            try
            {
                host.EnumDomains(out enumHandle);
                object domain = null;
                while (true)
                {
                    host.NextDomain(enumHandle, out domain);
                    if (domain == null)
                    {
                        break;
                    }
                    AppDomain appDomain = (AppDomain)domain;
                    if (appDomain.FriendlyName.Equals(friendlyName))
                    {
                        return appDomain;
                    }
                }
            }
            finally
            {
                host.CloseEnum(enumHandle);
                Marshal.ReleaseComObject(host);
                host = null;
            }
            return null;
        }

        public static T Instance
        {
            get
            {
                //如果当前对象为空，则去应用程序域中获取该对象
                if (null == _instance)
                {
                    //获取独立的应用程序域（该应用程序域用来保存全局信息）
                    AppDomain appDomain = GetAppDomain(AppDomainName);
                    if (null == appDomain)
                    {
                        //如果还没有该应用程序域则创建
                        AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                        setup.ApplicationName = "SingletonAppDomain";

                        // Set up the Evidence
                        Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);

                        appDomain = AppDomain.CreateDomain(AppDomainName, evidence, setup);
                    }
                    //根据传过来的类型去该应用程序中获取值
                    Type type = typeof(T);
                    T instance = (T)appDomain.GetData(type.FullName);
                    if (null == instance)
                    {
                        //如果在应用程序域中没有获取到该类型的值，则在这个应用程序域中创建该类型，并将当前的实例存入进去
                        instance = (T)appDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
                        appDomain.SetData(type.FullName, instance);
                    }
                    _instance = instance;
                }
                return _instance;
            }
            set
            {
                Type type = typeof(T);
                AppDomain appDomain = GetAppDomain(AppDomainName);
                appDomain.SetData(type.FullName, value);
            }
        }
    }
}
