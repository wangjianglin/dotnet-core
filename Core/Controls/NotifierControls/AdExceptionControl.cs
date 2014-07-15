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
    public class AdExceptionControl : Control
    {
        static AdExceptionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdExceptionControl), new FrameworkPropertyMetadata(typeof(AdExceptionControl)));
        }

        private Lin.Core.ViewModel.ViewModel vm = new Lin.Core.ViewModel.ViewModel();
        public readonly static DependencyProperty AdExceptionCodeProperty = DependencyProperty.Register("AdExceptionCode", typeof(string), typeof(AdExceptionControl), new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                //((AdExceptionControl)sender).vm.Property["ExceptionCode"] = e.NewValue;
            }));

        public string AdExceptionCode
        {
            set { this.SetValue(AdExceptionCodeProperty, value); }
            get { return (string)this.GetValue(AdExceptionCodeProperty); }
        }

        /// <summary>
        /// 错误提示信息
        /// </summary>
        public readonly static DependencyProperty AdExceptionInfoProperty = DependencyProperty.Register("AdExceptionInfo", typeof(string), typeof(AdExceptionControl),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
            }));

        public string AdExceptionInfo
        {
            set { this.SetValue(AdExceptionInfoProperty, value); }
            get { return (string)this.GetValue(AdExceptionInfoProperty); }
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
