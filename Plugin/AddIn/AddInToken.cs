using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Lin.Plugin.AddIn
{
    [Serializable]
    public class AddInToken:AttributeToken
    {
        private static string preKey = null;
        static string GetKey()
        {
            string key = DateTime.Now.Ticks + "";
            if (preKey != null)
            {
                while (key == preKey)
                {
                    System.Threading.Thread.Sleep(1);
                    key = DateTime.Now.Ticks + "";
                }
            }
            preKey = key;
            return key;
        }
        private string key = GetKey();

        public string AddInTypeName { get; private set; }
        public AddInToken(Type type)
        {
            AddInTypeName = GetKey();
            AppDomainVar.Vars[AddInTypeName] = type;
        }

        public Assembly AddInAssembly = null;
        public AddInToken()
        {

        }

        public string Name { get; set; }
        public AddInContentType AddInContentType { get; set; }
        private object _Content;
        public object Content
        {
            [System.STAThreadAttribute()]
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
            get
            {
                if (_Content == null)
                {                    
                    this.AddInAppDomain.DoCallBack(() =>
                    {
                        Type type = null;
                        if (AddInTypeName != null)
                        {
                            type = AppDomainVar.Vars[AddInTypeName] as Type;
                        }                        
                        if (this.AddInContentType == AddInContentType.CONTROL)
                        {
                            if (type != null)
                            {
                                INativeHandleContract ict = FrameworkElementAdapters.ViewToContractAdapter(Activator.CreateInstance(type) as FrameworkElement);
                                AppDomain.CurrentDomain.SetData(AddInTypeName, ict);
                            }
                            else
                            {
                                INativeHandleContract ict = FrameworkElementAdapters.ViewToContractAdapter(this.AddInAssembly.CreateInstance(this.ClassFullName) as FrameworkElement);
                                AppDomain.CurrentDomain.SetData(AddInTypeName, ict);
                            }
                        }
                        else if (AddInContentType == AddInContentType.EXCEUTE)
                        {
                            object obj = Activator.CreateInstance(type);
                            AppDomainVar.Vars[AddInTypeName] = obj;
                            ProxyExceute excu = new ProxyExceute(AddInTypeName);
                            AppDomain.CurrentDomain.SetData(AddInTypeName, excu);
                        }
                        else if (AddInContentType == Lin.Plugin.AddIn.AddInContentType.CONTENT)
                        {
                            AppDomain.CurrentDomain.SetData(AddInTypeName, Activator.CreateInstance(type));
                        }
                    });
                        
                    if (this.AddInContentType == AddInContentType.CONTROL)
                    {
                        INativeHandleContract iContract = AddInAppDomain.GetData(AddInTypeName) as INativeHandleContract;
                        _Content = FrameworkElementAdapters.ContractToViewAdapter(iContract);
                    }
                    else
                    {
                        _Content = AddInAppDomain.GetData(AddInTypeName);
                    }
                }
                return _Content;
            }
        }
        public AppDomain AddInAppDomain { get; internal set; }
        public string AddInName { get; internal set; }
        public string ClassFullName { get; internal set; }

        public string Description { get; internal set; }
        public string[] Params { get; internal set; }
        public string Publisher { get; internal set; }
        public string Version { get; internal set; }
        public string Type { get; internal set; }
        public double Location { get; internal set; }
        public uint Major { get; internal set; }
        public uint Minor { get; internal set; }
    }
}
