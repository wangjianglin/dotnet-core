using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lin.Core.Controls
{
    public class BusyControl : Control
    {
        static BusyControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyControl), new FrameworkPropertyMetadata(typeof(BusyControl)));
        }

        private class BusyControlViewModel:Lin.Core.ViewModel
        {
            BusyControl busy;
            public BusyControlViewModel(BusyControl c)
            {
                busy = c;
            }
            //protected override void OnCommandExecute(string command, object param)
            //{
            //    base.OnCommandExecute(command, param);
            //    switch(command)
            //    {
            //        case "CloseCommand":
            //            busy.Close(busy, new EventArgs());
            //            break;
            //    }                
            //}
        }

        private BusyControlViewModel vm ;

        public event System.EventHandler Close;

        public BusyControl()
        {
            vm = new BusyControlViewModel(this);
        }
        #region 依赖属性
        /// <summary>
        /// 是否显示Busy控件
        /// </summary>
        public readonly static DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyControl),
            new PropertyMetadata(false, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
               // ((BusyControl)sender).vm.Property["IsBusy"] = ((BusyControl)sender).IsBusy;
            }));
        public bool IsBusy
        {
            get { return (bool)this.GetValue(IsBusyProperty); }
            set { this.SetValue(IsBusyProperty, value); }
        }


        /// <summary>
        /// Busy控件上显示的内容
        /// </summary>
        public readonly static DependencyProperty BusyContentProperty = DependencyProperty.Register("BusyContent", typeof(string), typeof(BusyControl),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                //((BusyControl)sender).vm.Property["BusyContent"] = ((BusyControl)sender).BusyContent;
            }));
        public string BusyContent
        {
            get { return (string)this.GetValue(BusyContentProperty); }
            set { this.SetValue(BusyContentProperty, value); }
        }

        /// <summary>
        /// Busy控件显示的样式
        /// </summary>
        public readonly static DependencyProperty StyleThemeProperty = DependencyProperty.Register("StyleTheme", typeof(string), typeof(BusyControl),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                if (((BusyControl)sender).StyleTheme == "Metro")
                {
                    //((BusyControl)sender).vm.Property["MetroVisible"] = true;
                    //((BusyControl)sender).vm.Property["Windows7Visible"] = false;
                    //((BusyControl)sender).vm.Property["Windows8Visible"] = false;
                }
                if (((BusyControl)sender).StyleTheme == "Windows7")
                {
                    //((BusyControl)sender).vm.Property["MetroVisible"] = false;
                    //((BusyControl)sender).vm.Property["Windows7Visible"] = true;
                    //((BusyControl)sender).vm.Property["Windows8Visible"] = false;
                }
                if (((BusyControl)sender).StyleTheme == "Windows8")
                {
                    //((BusyControl)sender).vm.Property["MetroVisible"] = false;
                    //((BusyControl)sender).vm.Property["Windows7Visible"] = false;
                    //((BusyControl)sender).vm.Property["Windows8Visible"] = true;
                }
            }));
        public string StyleTheme
        {
            get { return (string)this.GetValue(StyleThemeProperty); }
            set { this.SetValue(StyleThemeProperty, value); }
        }

        /// <summary>
        /// Busy控件为进度条时，表示进度值
        /// </summary>
        public readonly static DependencyProperty ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(double), typeof(BusyControl),
            new PropertyMetadata(0.0, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                //((BusyControl)sender).vm.Property["ProgressValue"] = ((BusyControl)sender).ProgressValue * 1.81;
            }));
        public double ProgressValue
        {
            get { return (double)this.GetValue(ProgressValueProperty); }
            set { this.SetValue(ProgressValueProperty, value); }
        }

        /// <summary>
        /// 是否启用关闭功能
        /// </summary>
        public readonly static DependencyProperty IsCanCloseProperty = DependencyProperty.Register("IsCanClose", typeof(bool), typeof(BusyControl),
            new PropertyMetadata(true, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                if (((BusyControl)sender).IsCanClose)
                {
                    //((BusyControl)sender).vm.Property["IsCanClose"] = Visibility.Visible;
                }
                else
                {
                    //((BusyControl)sender).vm.Property["IsCanClose"] = Visibility.Hidden;
                }
            }));
        public bool IsCanClose
        {
            get { return (bool)this.GetValue(IsCanCloseProperty); }
            set { this.SetValue(IsCanCloseProperty, value); }
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            FrameworkElement d = null;
            for (int n = 0; n < this.VisualChildrenCount; n++)
            {
                d = this.GetVisualChild(n) as FrameworkElement;
                if (d != null)
                {
                    d.DataContext = vm;
                }
            }
        }
    }
}
