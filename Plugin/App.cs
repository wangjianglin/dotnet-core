using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Lin.Plugin
{
    public static class App
    {
        public static void Shutdown(int exitCode=0)
        {
            Utils.GetDefaultAppDomain().DoCallBack(() =>
            {
                System.Windows.Application.Current.Shutdown();
            });
        }
    }
}
