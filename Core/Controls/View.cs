using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Lin.Core.AddIn;

#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "Lin.Core.Controls")]

#endregion

namespace Lin.Core.Controls
{
    /// <summary>
    /// 是所有页面视图的基类，
    /// </summary>
    public abstract class View : Control
    {
        /// <summary>
        /// 视图中的组成项，
        /// </summary>
        public static readonly DependencyProperty ViewItemsProperty = DependencyProperty.RegisterAttached("ViewItems", typeof(ViewItems), typeof(View),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                FrameworkElement fe = d as FrameworkElement;
                if (fe != null)
                {
                    ViewItems items = fe.GetValue(View.ViewItemsProperty) as ViewItems;
                    if (items != null && items.DataContext == null)
                    {
                        items.AttachedObject = fe;
                        items.DataContext = fe.DataContext;
                    }
                }
            }));

        public static ViewItems GetViewItems(DependencyObject obj)
        {
            return (ViewItems)obj.GetValue(ViewItemsProperty);
        }

        public static void SetViewItems(DependencyObject obj, ViewItems value)
        {
            obj.SetValue(ViewItemsProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        private static IList<AddInToken> tokens;

        static View()
        {
            Lin.Core.Config.IConfigManager netConfig = Lin.Core.Config.ConfigManager.System;
            //string path = Environment.CurrentDirectory;
            string path = Lin.Plugin.Utils.GetRunDir(1, true);
            AddInStore.Update(path);

            tokens = AddInStore.FindAddIns();
            if (tokens == null)
            {
                tokens = new List<AddInToken>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public View()
        {
            this.AddIns = new List<AddInToken>(tokens);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public View(string type)
        {
            this.AddIns = new List<AddInToken>();
            if (type == "")
            {
                type = null;
            }
            foreach (AddInToken token in tokens)
            {
                if (token.Type == type)
                {
                    this.AddIns.Add(token);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<AddInToken> AddIns
        {
            get
            {
                return (IList<AddInToken>)GetValue(AddInsProperty);
            }
            private set
            {
                SetValue(AddInsProperty, value);
            }
        }

        public static readonly DependencyProperty AddInsProperty =
        DependencyProperty.Register("AddIns", typeof(IList<AddInToken>), typeof(View), new PropertyMetadata(null));

        public object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }
            protected set
            {
                SetValue(ContentProperty, value);
            }
        }
        public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(object), typeof(View), new PropertyMetadata(null,
            (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                View view = d as View;
                object obj1 = (view.Content as System.Windows.DependencyObject).GetValue(Lin.Core.Controls.View.ViewItemsProperty);
                if (obj1 == null)
                {
                    return;
                }
                object obj2 = view.GetValue(Lin.Core.Controls.View.ViewItemsProperty);
                if (obj2 != null)
                {
                    ViewItems v1 = obj1 as ViewItems;
                    ViewItems v2 = obj2 as ViewItems;
                    v2["Menu"] = v1["Menu"];
                    view.SetValue(Lin.Core.Controls.View.ViewItemsProperty, v2);
                }
                else
                {
                    ViewItems v = new ViewItems();
                    v["Menu"] = (obj1 as ViewItems)["Menu"];
                    view.SetValue(Lin.Core.Controls.View.ViewItemsProperty, v);
                }
            }
            ));

        /// <summary>
        /// 获取最新的版本号
        /// </summary>
        /// <returns></returns>
        protected AddInToken GetLaster()
        {
            AddInToken add = null;

            if (AddIns != null && AddIns.Count > 0)
            {
                add = this.AddIns[0];
                for (int i = 1; i < this.AddIns.Count; i++)
                {
                    if (add.Major < this.AddIns[i].Major || (add.Major == this.AddIns[i].Major && add.Minor < this.AddIns[i].Minor))
                    {
                        add = this.AddIns[i];
                    }
                }
            }
            return add;
        }
    }
}
