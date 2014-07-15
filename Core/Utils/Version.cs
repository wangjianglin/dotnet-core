using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lin.Core.Utils
{
    public static class Version
    {
        /// <summary>
        /// 获取当前运行的EXE程序的版本信息
        /// </summary>
        /// <returns></returns>
        public static FileVersionInfo GetNowVersion()
        {
            Process pro = Process.GetCurrentProcess(); // 获取当前进程
            string proPath = pro.MainModule.FileName;
            FileVersionInfo file = System.Diagnostics.FileVersionInfo.GetVersionInfo(proPath);
            return file;
        }
        /// <summary>
        /// 获取当前运行的EXE程序的版本信息
        /// </summary>
        /// <returns></returns>
        public static FileVersionInfo GetNowVersion(string proName)
        {
            Process[] process = Process.GetProcessesByName(proName); // 获取当前进程
            Process pro = process[0];
            string proPath = pro.MainModule.FileName;
            FileVersionInfo file = System.Diagnostics.FileVersionInfo.GetVersionInfo(proPath);
            return file;
        }
    }
}
