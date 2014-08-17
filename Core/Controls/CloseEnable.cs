using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Lin.Core.Controls
{
    public abstract class CloseEnable : Control, ICloseEnable
    {
        public CloseEnable()
        {
            ///初始化的时候设置取消和关闭命令
            this.IsVisibleChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
            {
                Lin.Core.ViewModel view = vm as Lin.Core.ViewModel;
                if(view!=null)
                {
                    //view.Property["Closed"] = false;
                    //view.Property["Cancel"] = false;
                } 
            };
            this.DataContextChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
            {
                this.Binding(this.DataContext);
            };
        }
        private object vm;

        protected void Binding(object source)
        {
            vm = source;
            Binding bind = null;
            
            //绑定 返回值
            bind = new Binding("Property[Result]");
            bind.Source = source;
            bind.Mode = BindingMode.OneWay;
            this.SetBinding(ResultProperty, bind);

            //绑定关闭
            bind = new Binding("Property[Closed]");
            bind.Source = source;
            bind.Mode = BindingMode.OneWay;
            this.SetBinding(ClosedProperty, bind);

            //绑定 取消
            bind = new Binding("Property[Cancel]");
            bind.Source = source;
            bind.Mode = BindingMode.OneWay;
            this.SetBinding(CancelProperty, bind);
             
            object tmpValue = this.GetValue(Dialog.ParamsProperty);

            this.SetValue(CloseEnable.ParamsProperty, null);
            //绑定 参数
            bind = new Binding("Property[Params]");
            bind.Source = source;
            bind.Mode = BindingMode.OneWayToSource;
            this.SetBinding(CloseEnable.ParamsProperty, bind);

            this.SetValue(CloseEnable.ParamsProperty, tmpValue);
            //this.OnPropertyChanged(new DependencyPropertyChangedEventArgs(CloseEnable.ParamsProperty, null, tmpValue));
        } 
        private static readonly DependencyProperty ParamsProperty = DependencyProperty.Register("Params", typeof(object), typeof(CloseEnable),
          null); 
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name == "Params" && e.Property.OwnerType == typeof(Dialog))
            {
                this.SetValue(CloseEnable.ParamsProperty, e.NewValue);
            }
        }
        private static readonly DependencyProperty ClosedProperty = DependencyProperty.Register("Closed", typeof(bool), typeof(CloseEnable),
            new PropertyMetadata(false, (DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
            {
                if (true.Equals(args.NewValue))
                {
                    ((CloseEnable)sender).Close();
                }
            }));

        private static readonly DependencyProperty CancelProperty = DependencyProperty.Register("Cancel", typeof(bool), typeof(CloseEnable),
           new PropertyMetadata(false, (DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
           {
               if (true.Equals(args.NewValue))
               {
                   ((CloseEnable)sender).Cancel();
               }
           }));

        private static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof(object), typeof(CloseEnable),
          new PropertyMetadata(false, (DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
          {
              ((CloseEnable)sender).Result = args.NewValue;
          }));

        protected string OkButtonName = "PATH_OK_Button";
        protected string CancelButtonName = "PATH_Cancel_Button";
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button ok = this.GetTemplateChild(OkButtonName) as Button;
            if (ok != null)
            {
                ok.Click += (object sender, RoutedEventArgs args) =>
                {
                    if (Closed != null)
                    {
                        Closed(this, new EventArgs());
                    }
                };
            }
            Button cancel = this.GetTemplateChild(CancelButtonName) as Button;
            if (cancel != null)
            {
                cancel.Click += (object sender, RoutedEventArgs args) =>
                {
                    if (Canceled != null)
                    {
                        Canceled(this, new EventArgs());
                    }
                };
            } 
            FrameworkElement fe = this.GetVisualChild(0) as FrameworkElement;
            if (fe != null)
            {
                fe.DataContextChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
                {
                    this.Binding(e.NewValue);
                };
            }

            //Lin.Core.ViewModel.ViewModel tmpvm = vm as Lin.Core.ViewModel.ViewModel;

            //if (tmpvm != null)
            //{
            //    tmpvm.Property["Window"] = Dialog.GetWindow(this);
            //}
            //else
            //{
            //    return;
            //}
        }


        public void Cancel()
        {
            if (Canceled != null)
            {
                Canceled(this, new EventArgs());
            }
        }
        public void Close()
        {
            if (Closed != null)
            {
                Closed(this, new EventArgs());
            }
        }
        public event EventHandler Closed;
        public event EventHandler Canceled;

        private object _result;

        /// <summary>
        /// 当返回结果发生变化时
        /// </summary>
        public event Action<object> ResultChanged;
        /// <summary>
        /// 返回结果
        /// </summary>
        public virtual object Result
        {
            get { return _result; }
            protected set { this._result = value; if (ResultChanged != null) { ResultChanged(_result); } }
        }

        /// <summary>
        /// 关闭窗口行为(直接关闭或关闭且返回结果)
        /// </summary>
        private WindowClosedBehavior _windowClosedBehavior;
        public virtual WindowClosedBehavior WindowClosedBehavior
        {
            get { return this._windowClosedBehavior; }
            protected set { this._windowClosedBehavior = value; }
        }
    }
}
