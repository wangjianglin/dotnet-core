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

namespace Lin.Core.Controls.NotifierControls
{
    public class ErrorControl : Control
    {
        static ErrorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ErrorControl), new FrameworkPropertyMetadata(typeof(ErrorControl)));
        }

        private Lin.Core.ViewModel.ViewModel vm = new Lin.Core.ViewModel.ViewModel();
        public readonly static DependencyProperty ErrorCodeProperty = DependencyProperty.Register("ErrorCode", typeof(string), typeof(ErrorControl), new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
        {
            //((ErrorControl)sender).vm.Property["ErrorCode"] = e.NewValue;
        }));

        public string ErrorCode
        {
            set { this.SetValue(ErrorCodeProperty, value); }
            get { return (string)this.GetValue(ErrorCodeProperty); }
        }

        /// <summary>
        /// 错误提示信息
        /// </summary>
        public readonly static DependencyProperty ErrorInfoProperty = DependencyProperty.Register("ErrorInfo", typeof(string), typeof(ErrorControl),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
            }));

        public string ErrorInfo
        {
            set { this.SetValue(ErrorInfoProperty, value); }
            get { return (string)this.GetValue(ErrorInfoProperty); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            FrameworkElement fe = this.GetVisualChild(0) as FrameworkElement;
            if (fe != null)
            {
                fe.DataContext = vm;
            }
            else
            {
                this.DataContext = vm;
            }
        }
    }
}
