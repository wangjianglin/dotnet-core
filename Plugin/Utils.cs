using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace Lin.Plugin
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 查找到当前代码运行的目录
        /// </summary>
        /// <param name="frame">查找堆栈的层数</param>
        /// <param name="isFindUpLive"></param>
        /// <param name="findLive"></param>
        /// <returns></returns>
        public static string GetRunDir(int frame = 1, bool isFindUpLive = false, string findLive = null)
        {
            System.Diagnostics.StackTrace s = new System.Diagnostics.StackTrace();
            string fullName2 = null;
            if (findLive == null)
            {
                fullName2 = s.GetFrame(0).GetMethod().DeclaringType.Assembly.CodeBase;
            }
            else
            {
                fullName2 = findLive;
            }

            string fullName = null;
            int count = 0;
            if (frame > 0)
            {
                if (findLive == null)
                {
                    for (int n = 0; ; n++)
                    {
                        fullName = s.GetFrame(n).GetMethod().DeclaringType.Assembly.CodeBase;
                        if (fullName != fullName2)
                        {
                            if (frame <= ++count)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int n = 2; ; n++)
                    {
                        fullName = s.GetFrame(n).GetMethod().DeclaringType.Assembly.CodeBase;
                        if (fullName != fullName2)
                        {
                            if (frame <= ++count)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                fullName = s.GetFrame(0).GetMethod().DeclaringType.Assembly.CodeBase;
            }
            if (isFindUpLive)
            {
                fullName = GetRunDir(1,false,fullName);
            }
            string str = "///";
            int index = fullName.IndexOf(str);
            int lastIndex = fullName.LastIndexOf('/') + 1;
            return fullName.Substring(index + str.Length, lastIndex - index - str.Length);
        }

        public static AppDomain[] GetAppDomains()
        {
            return null;
        }

        /// <summary>
        /// 获取主域
        /// </summary>
        /// <returns></returns>
        public static AppDomain GetDefaultAppDomain()
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
                    if (appDomain.IsDefaultAppDomain())
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

        public static AppDomain GetAppDomain(string friendlyName)
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


        //
        // 摘要:
        //     使用指定的名称新建应用程序域。
        //
        // 参数:
        //   friendlyName:
        //     域的友好名称。
        //
        // 返回结果:
        //     新创建的应用程序域。
        //
        // 异常:
        //   System.ArgumentNullException:
        //     friendlyName 为 null。
        public static AppDomain CreateDomain(string friendlyName)
        {
            return null;
        }
        //
        // 摘要:
        //     使用所提供的证据创建具有给定名称的新应用程序域。
        //
        // 参数:
        //   friendlyName:
        //     域的友好名称。此友好名称可在用户界面中显示以标识域。有关更多信息，请参见System.AppDomain.FriendlyName。
        //
        //   securityInfo:
        //     确定代码标识的证据，该代码在应用程序域中运行。传递 null 以使用当前应用程序域的证据。
        //
        // 返回结果:
        //     新创建的应用程序域。
        //
        // 异常:
        //   System.ArgumentNullException:
        //     friendlyName 为 null。
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo)
        {
            return null;
        }
        //
        // 摘要:
        //     使用指定的名称、证据和应用程序域设置信息创建新的应用程序域。
        //
        // 参数:
        //   friendlyName:
        //     域的友好名称。此友好名称可在用户界面中显示以标识域。有关更多信息，请参见System.AppDomain.FriendlyName。
        //
        //   securityInfo:
        //     确定代码标识的证据，该代码在应用程序域中运行。传递 null 以使用当前应用程序域的证据。
        //
        //   info:
        //     包含应用程序域初始化信息的对象。
        //
        // 返回结果:
        //     新创建的应用程序域。
        //
        // 异常:
        //   System.ArgumentNullException:
        //     friendlyName 为 null。
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup info)
        {
            return null;
        }
        //
        // 摘要:
        //     使用指定的名称、证据、应用程序域设置信息、默认权限集和一组完全受信任的程序集创建新的应用程序域。
        //
        // 参数:
        //   friendlyName:
        //     域的友好名称。此友好名称可在用户界面中显示以标识域。有关更多信息，请参见 System.AppDomain.FriendlyName 的说明。
        //
        //   securityInfo:
        //     确定代码标识的证据，该代码在应用程序域中运行。传递 null 以使用当前应用程序域的证据。
        //
        //   info:
        //     包含应用程序域初始化信息的对象。
        //
        //   grantSet:
        //     一个默认权限集，被授予加载到新应用程序域的所有无特定权限的程序集。
        //
        //   fullTrustAssemblies:
        //     一组强名称，表示在新应用程序域中被认为完全受信任的程序集。
        //
        // 返回结果:
        //     新创建的应用程序域。
        //
        // 异常:
        //   System.ArgumentNullException:
        //     friendlyName 为 null。
        //
        //   System.InvalidOperationException:
        //     应用程序域为 null。- 或 -System.AppDomainSetup.ApplicationBase 属性在为 info 提供的 System.AppDomainSetup
        //     对象上没有设置。
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup info, PermissionSet grantSet, params StrongName[] fullTrustAssemblies)
        {
            return null;
        }
        //
        // 摘要:
        //     使用证据、应用程序基路径、相对搜索路径和指定是否向应用程序域中加载程序集的影像副本的形参创建具有给定名称的新应用程序域。
        //
        // 参数:
        //   friendlyName:
        //     域的友好名称。此友好名称可在用户界面中显示以标识域。有关更多信息，请参见System.AppDomain.FriendlyName。
        //
        //   securityInfo:
        //     确定代码标识的证据，该代码在应用程序域中运行。传递 null 以使用当前应用程序域的证据。
        //
        //   appBasePath:
        //     基目录，由程序集冲突解决程序用来探测程序集。有关更多信息，请参见System.AppDomain.BaseDirectory。
        //
        //   appRelativeSearchPath:
        //     相对于基目录的路径，在此程序集冲突解决程序应探测专用程序集。有关更多信息，请参见System.AppDomain.RelativeSearchPath。
        //
        //   shadowCopyFiles:
        //     如果为 true，则向此应用程序域中加载程序集的影像副本。有关更多信息，请参见 System.AppDomain.ShadowCopyFiles
        //     和Shadow Copying Assemblies。
        //
        // 返回结果:
        //     新创建的应用程序域。
        //
        // 异常:
        //   System.ArgumentNullException:
        //     friendlyName 为 null。
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles)
        {
            return AppDomain.CreateDomain(friendlyName, securityInfo, appBasePath, appRelativeSearchPath, shadowCopyFiles);
        }

        public static void LoadFrom(AppDomain appDomain,string file)
        {
            appDomain.SetData(Thread.CurrentThread.ManagedThreadId + "LoadFile", file);
            appDomain.DoCallBack(() =>
            {
                Assembly ass2 = Assembly.LoadFrom((string)AppDomain.CurrentDomain.GetData(Thread.CurrentThread.ManagedThreadId + "LoadFile"));
            });
        }
        //
        // 摘要:
        //     使用证据、应用程序基路径、相对搜索路径和指定是否向应用程序域中加载程序集的影像副本的形参创建具有给定名称的新应用程序域。指定在初始化应用程序域时调用的回调方法，以及传递回调方法的字符串实参数组。
        //
        // 参数:
        //   friendlyName:
        //     域的友好名称。此友好名称可在用户界面中显示以标识域。有关更多信息，请参见System.AppDomain.FriendlyName。
        //
        //   securityInfo:
        //     确定代码标识的证据，该代码在应用程序域中运行。传递 null 以使用当前应用程序域的证据。
        //
        //   appBasePath:
        //     基目录，由程序集冲突解决程序用来探测程序集。有关更多信息，请参见System.AppDomain.BaseDirectory。
        //
        //   appRelativeSearchPath:
        //     相对于基目录的路径，在此程序集冲突解决程序应探测专用程序集。有关更多信息，请参见System.AppDomain.RelativeSearchPath。
        //
        //   shadowCopyFiles:
        //     如果为 true，则将程序集的影像副本加载到应用程序域中。有关更多信息，请参见 System.AppDomain.ShadowCopyFiles
        //     和Shadow Copying Assemblies。
        //
        //   adInit:
        //     System.AppDomainInitializer 委托，表示初始化新的 System.AppDomain 对象时调用的回调方法。
        //
        //   adInitArgs:
        //     字符串实参数组，在初始化新的 System.AppDomain 对象时传递给由 adInit 表示的回调。
        //
        // 返回结果:
        //     新创建的应用程序域。
        //
        // 异常:
        //   System.ArgumentNullException:
        //     friendlyName 为 null。
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles, AppDomainInitializer adInit, string[] adInitArgs)
        {
            return null;
        }
        
    }
}
