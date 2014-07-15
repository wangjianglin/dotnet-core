using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Lin.Core.Controls;

namespace Lin.Core.AddIn
{
    public class AddInToken
    {
        private Type _AddInType;
        public AddInToken(Type type)
        {
            _AddInType = type;
        }

        public string Name { get; internal set; }
        private object _Content;
        public object Content
        {
            get
            {
                if (_Content == null)
                {
                    _Content = Activator.CreateInstance(_AddInType);
                    //FrameworkElement fe = _Content as FrameworkElement;
                    //if (fe != null)
                    //{
                    //    ViewItems items = fe.GetValue(View.ViewItemsProperty) as ViewItems;
                    //    if (items != null && items.DataContext == null)
                    //    {
                    //        items.AttachedObject = fe;
                    //        items.DataContext = fe.DataContext;
                    //    }
                    //}
                }
                return _Content;
            }
        }
        public string Description { get; internal set; }
        public string[] Params { get; set; }
        public string Publisher { get; internal set; }
        public string Version { get; internal set; }
        public string Type { get; internal set; }
        public double Location { get; internal set; }
        public uint Major { get; set; }
        public uint Minor { get; set; }
    }
}
