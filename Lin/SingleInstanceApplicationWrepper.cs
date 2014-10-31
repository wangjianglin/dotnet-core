using Lin.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AD.ECM
{
    /// <summary>
    /// 实现单实例运行
    /// </summary>
    public class SingleInstanceApplicationWrepper :Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        public SingleInstanceApplicationWrepper()
        {
            this.IsSingleInstance = true;
        }
        
        static SingleInstanceApplicationWrepper()
        {
            string[] regeditCommandLines = null;
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\EPCBM");
            if (reg != null)
            {
                string args = (string)reg.GetValue("args");
                if (args != null)
                {
                    regeditCommandLines = DisposeRegedit(args).ToArray();
                }
            }
            else
            {
                Microsoft.Win32.RegistryKey local= Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EPCBM");
                if (local != null)
                {
                    string args = (string)local.GetValue("args");
                    if (args != null)
                    {
                        regeditCommandLines = DisposeRegedit(args).ToArray();
                    }
                }
            }

            string[] tempCommandLines = Environment.GetCommandLineArgs();
            string[] tmpArgs = new string[tempCommandLines.Length - 1];
            for (int i = 0; i < tempCommandLines.Length - 1; i++)
            {
                tmpArgs[i] = tempCommandLines[i + 1];
            }
            tempCommandLines = tmpArgs;

            //当注册表命令行参数为空，只有程序启动命令行参数时
            if (regeditCommandLines == null && tempCommandLines.Length > 0) //|| regeditCommandLines == null && tempCommandLines.Length < 1)
            {
                CommandLineArguments.ProcessArgs(DisposeLineArgs(tempCommandLines).ToArray());
            }
            //当只有注册表命令行参数时，直接处理命令行参数
            if (regeditCommandLines != null && tempCommandLines.Length < 1)
            {
                CommandLineArguments.ProcessArgs(regeditCommandLines);
            }
            //当注册表命令行参数、程序启动命令行参数都不为空时
            if (regeditCommandLines != null && tempCommandLines.Length > 0)
            {
                //在注册表命令行参数列表中，找到与程序启动命令行参数以“--D”开头的相同的命令行参数,并记录相同命令行参数在注册表集合中的位置
                List<int> indexs = new List<int>();
                for (int i = 0; i < regeditCommandLines.Length; i++)
                {
                    if (regeditCommandLines[i].StartsWith("-"))
                    {
                        for (int j = 0; j < tempCommandLines.Length; j++)
                        {
                            if (tempCommandLines[j].StartsWith("--D"))
                            {
                                if (regeditCommandLines[i].Substring(1) == tempCommandLines[j].Substring(3))
                                {
                                    indexs.Add(i);
                                    if (i < regeditCommandLines.Length - 1)
                                    {
                                        if (!regeditCommandLines[i + 1].StartsWith("-"))
                                        {
                                            indexs.Add(i + 1);
                                            i++;
                                            break;
                                        }
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                //有相同的命令行参数时，根据记录的位置将注册表中相同的命令行参数去除，只处理没有标记的参数(当启动参数加了强制默认命令行参数"--D"标志时则将这个参数作为默认参数处理)
                if (indexs.Count > 0)
                {
                    List<string> tmpprocess = new List<string>();
                    for (int i = 0; i < regeditCommandLines.Length; i++)
                    {
                        if (!indexs.Contains(i))
                        {
                            tmpprocess.Add(regeditCommandLines[i]);
                        }
                    }
                    CommandLineArguments.ProcessArgs(tmpprocess.ToArray());
                }
                else
                {
                    CommandLineArguments.ProcessArgs(regeditCommandLines);
                }

                //处理程序启动命令行参数(DisposeLineArgs方法返回的是加了强制默认命令行参数"--D"标志的命令行参数)
                CommandLineArguments.ProcessArgs(DisposeLineArgs(tempCommandLines).ToArray());
            }
        }
        private App app;
        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs eventArgs)
        {
            app = new App();
            app.Run();
            return false; 
        }
        protected override void OnStartupNextInstance(Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs eventArgs)
        {
            //处理  eventArgs.CommandLine
            CommandLineArguments.ProcessArgs(eventArgs.CommandLine.ToArray(), AccessOpportunity.OnStartupNextInstance);

            //IList<string> Slients = CommandLineArguments.NextArgs["Slient"] as IList<string>;

            //Exceute.Exceute(Lin.Plugin.ApplicationLife.ApplicationLifecyclePhase.ENTER_STARTUP_NEXT_INSTANCE);
            //base.OnStartupNextInstance(eventArgs);
            //Exceute.Exceute(Lin.Plugin.ApplicationLife.ApplicationLifecyclePhase.LEAVE_STARTUP_NEXT_INSTANCE);

            ////发布
            //CommandLineArguments.Fire(AccessOpportunity.OnStartupNextInstance);
             
            ////清除参数
            //CommandLineArguments.ClearNextCommandArgs();

            //if (app != null && app.MainWindow.IsActive == true)
            //{
            //    if (app.MainWindow.WindowState == System.Windows.WindowState.Minimized)
            //    {
            //        app.MainWindow.WindowState = System.Windows.WindowState.Normal;
            //    }
            //    app.MainWindow.Activate();
            //    app.MainWindow.Visibility = Visibility.Visible;
            //}
           
        }

        public static Lin.Plugin.Plugins appPlugins = null;
        public static IExceute Exceute;
        [STAThread]
        public static void Main(string[] args)
        {
            //Environment.CurrentDirectory = global::System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            ////正常启动程序
            //appPlugins = new Lin.Plugin.Plugins(new global::System.IO.DirectoryInfo(global::System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Plugins"));

            //try
            //{
            //    Plugin.AddIn.AddInToken apps = FindExcuteAddin();
            //    Exceute = apps.Content as IExceute;
            //}
            //catch
            //{
            //    MessageBox.Show("应用程序启动失败！");
            //}
            //Exceute.Exceute(Lin.Plugin.ApplicationLife.ApplicationLifecyclePhase.ENTER_MAIN);

            
            //Environment.CurrentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;           
            //SingleInstanceApplicationWrepper wrapper = new SingleInstanceApplicationWrepper();
            //wrapper.Run(args);
        }

        /// <summary>
        /// 根据命令行参数查找对应版本的执行器
        /// </summary>
        /// <returns></returns>
        //private static Lin.Plugin.AddIn.AddInToken FindExcuteAddin()
        //{
        //    Lin.Plugin.AddIn.AddInToken apps = null;

        //    IList<string> appsVersions = CommandLineArguments.Args["App"] as IList<string>;
        //    IList<Lin.Plugin.AddIn.AddInToken> addins = null;
        //    if (appsVersions != null)
        //    {
        //        addins = appPlugins.GetAddIns("App Core", typeof(Lin.Plugin.IExceute));
        //        IList<string> appEnforces = CommandLineArguments.Args["AppEnforce"] as IList<string>;
        //        if (appEnforces != null)
        //        {
        //            string appEnforce = appEnforces[0];
        //            uint major = (uint)int.Parse(appEnforce.Split('.')[0]);
        //            uint minor = (uint)int.Parse(appEnforce.Split('.')[1]);
        //            foreach (Lin.Plugin.AddIn.AddInToken item in addins)
        //            {
        //                if (item.Major == major && item.Minor == minor)
        //                {
        //                    apps = item;
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            string appsVersion = appsVersions[0];
        //            uint major = (uint)int.Parse(appsVersion.Split('.')[0]);
        //            uint minor = (uint)int.Parse(appsVersion.Split('.')[1]);
        //            uint preMajor = uint.MaxValue;
        //            uint preMinor = uint.MaxValue;
        //            foreach (Lin.Plugin.AddIn.AddInToken item in addins)
        //            {
        //                if (item.Major == major && item.Minor == minor)
        //                {
        //                    apps = item;
        //                    break;
        //                }
        //            }
        //            if (apps == null)
        //            {
        //                foreach (Lin.Plugin.AddIn.AddInToken item in addins)
        //                {
        //                    if ((item.Major > major || item.Major == major && item.Minor >= minor)
        //                        &&
        //                        (item.Major < preMajor || item.Major == major && item.Minor < preMinor))
        //                    {
        //                        apps = item;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        apps = appPlugins.GetLastAddIn("App Core", typeof(Lin.Plugin.IExceute));
        //    }

        //    return apps;
        //}

        /// <summary>
        /// 处理来自注册表的命令行参数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static List<string> DisposeRegedit(string args)
        {
            List<string> arg = new List<string>();
            string[] commandlines=null;
            string tmpargs = null;
            if (args.Contains("\\-"))
            {
                tmpargs = args.Replace("\\-", "045");
                commandlines = tmpargs.Split('-');
            }
            else
            {
                commandlines = args.Split('-');
            }

             
            for (int i = 0; i < commandlines.Length; i++)
            {
                if (commandlines[i] != null && commandlines[i] != "")
                {
                    string commandline = commandlines[i];
                    if (commandline.Contains('"'))
                    {
                        string[] values = commandline.Split('"');
                        arg.Add(values[0].Insert(0, "-"));
                        StringBuilder line = new StringBuilder();
                        for (int j = 1; j < values.Length; j++)
                        {
                            line.Append(values[j]);
                        }
                        if (line.ToString().Contains("045"))
                        {
                            arg.Add(line.ToString().Replace("045", "\\-"));
                        }
                        else
                        {
                            arg.Add(line.ToString());
                        }
                    }
                    else
                    {
                        string[] values = commandline.Split(' ');
                        arg.Add(values[0].Insert(0, "-"));
                        if (values.Length > 1)
                        {
                            if (values[1] != "" && values[1] != " ")
                            {
                                if (values[1].Contains("045"))
                                {
                                    arg.Add(values[1].Replace("045", "\\-"));
                                }
                                else
                                {
                                    arg.Add(values[1]);
                                }
                            }
                        }
                    }
                }
            }
            return arg;
        }

        /// <summary>
        /// 处理来自程序启动时所带的命令行参数(会将临时参数处理)
        /// </summary>
        /// <param name="tempCommandLines">从程序启动时获取到的命令行参数</param>
        /// <returns>返回已"--D"开头的特殊强制默认参数列表集合</returns>
        private static List<string> DisposeLineArgs(string[] tempCommandLines)
        {
            List<string> argsCommandLines = new List<string>();
            List<int> indexs = new List<int>();

            for (int i = 0; i < tempCommandLines.Length; i++)
            {
                if (tempCommandLines[i].StartsWith("--D"))
                {
                    argsCommandLines.Add(tempCommandLines[i].Substring(3).Insert(0, "-"));
                    indexs.Add(i);
                    if (i < tempCommandLines.Length - 1)
                    {
                        if (!tempCommandLines[i + 1].StartsWith("-"))
                        {
                            argsCommandLines.Add(tempCommandLines[i + 1]);
                            indexs.Add(i + 1);
                            i++;
                        }
                    }
                }
            }

            if (indexs.Count > 0)
            {
                List<string> manageArgs = new List<string>();
                for (int i = 0; i < tempCommandLines.Length; i++)
                {
                    if (!indexs.Contains(i))
                    {
                        manageArgs.Add(tempCommandLines[i]);
                    }
                }
                CommandLineArguments.ProcessArgs(manageArgs.ToArray(), AccessOpportunity.OnStartupNextInstance);
            }
            else
            {
                CommandLineArguments.ProcessArgs(tempCommandLines, AccessOpportunity.OnStartupNextInstance);
            }

            return argsCommandLines;
        }
    }
}
