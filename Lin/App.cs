using System;
using System.Windows;
using System.Globalization;
using System.Windows.Threading;
using System.Diagnostics;
using Lin.Plugin;

namespace AD.ECM
{
    public class App : System.Windows.Application
    {
        public App()
        {
#if RELEASE
            Process[] prc = Process.GetProcesses();
            foreach (Process pr in prc) //遍历整个进程
            {
                if (pr.ProcessName == "Update") //如果进程存在
                {
                    pr.Kill();
                }
            }
           Process.Start(Environment.CurrentDirectory + "\\AutoUpdate\\Update.exe");
#endif
           // (SingleInstanceApplicationWrepper.appPlugins.GetLastAddIn("Core") as IExceute).Exceute(this);
            SingleInstanceApplicationWrepper.Exceute.Exceute(Lin.Plugin.ApplicationLife.ApplicationLifecyclePhase.ENTER_APP);
            SingleInstanceApplicationWrepper.Exceute.Exceute(Lin.Plugin.ApplicationLife.ApplicationLifecyclePhase.LEAVE_APP);

#if RELEASE
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
#else
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;
#endif
            //LocalizationManager.DefaultCulture = new CultureInfo("zh-CN");
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            SingleInstanceApplicationWrepper.Exceute.Exceute(Lin.Plugin.ApplicationLife.ApplicationLifecyclePhase.ENTER_ON_STARTUP);
            base.OnStartup(e);
            SingleInstanceApplicationWrepper.Exceute.Exceute(Lin.Plugin.ApplicationLife.ApplicationLifecyclePhase.LEAVE_ON_STARTUP);
            CommandLineArguments.Fire(AccessOpportunity.OnStartup);
        }
    }
}

