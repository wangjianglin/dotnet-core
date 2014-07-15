using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;

namespace Lin.Core
{
    public static class GetAppDomain
    {
        /// <summary>
        /// 根据指定的名称获取应用程序域
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        private static object lockObj = new object();
        public static AppDomain GetSingleDomainAndCreate()
        {
            lock (lockObj)
            {
                string friendlyName = "SingleAppDomain";
                AppDomain appDomain = null;
                appDomain = Lin.Plugin.Utils.GetAppDomain(friendlyName);
                if (appDomain != null)
                {
                    return appDomain;
                }
                // 如果还没有该应用程序域则创建
                AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                setup.ApplicationName = friendlyName;
                // 制定凭证
                Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);

                //appDomain = AppDomain.CreateDomain(friendlyName, evidence, setup);


                appDomain = Lin.Plugin.Utils.CreateDomain(friendlyName, AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    "", true);
                string dllPath = Assembly.GetCallingAssembly().CodeBase;
                Lin.Plugin.Utils.LoadFrom(appDomain, dllPath);
                return appDomain;
            }
        }
    }
}
