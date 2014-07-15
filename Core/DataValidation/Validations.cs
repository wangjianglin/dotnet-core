using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Lin.Core.DataValidation
{
    /// <summary>
    /// 数据验证附加属性
    /// </summary>
    public class Validations
    {

        /// <summary>
        /// 是否使用数据验证
        /// </summary>
        public static readonly DependencyProperty CatchErrorProperty = DependencyProperty.RegisterAttached("CatchError", typeof(bool), typeof(Validations),
            new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) => 
            {
                System.Windows.Controls.Validation.AddErrorHandler(d, validationError);
                //FrameworkElement fe = d as FrameworkElement;
                //if (fe != null)
                //{
                //    fe.Unloaded += (object sender, RoutedEventArgs e2) =>
                //    {
                //        System.Windows.Controls.Validation.RemoveErrorHandler(d, validationError);
                //    };
                //}
            }));
        public static void SetCatchError(DependencyObject obj, bool value)
        {
            obj.SetValue(HasErrorProperety, value);
        }
        public static bool GetCatchError(DependencyObject obj)
        {
            return (bool)obj.GetValue(CatchErrorProperty);
        }

        /// <summary>
        /// 使用数据验证后，对数据进行验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void validationError(object sender, ValidationErrorEventArgs args)
        {
            if ((sender as FrameworkElement).DataContext != null && (sender as FrameworkElement).DataContext as IDataValidationInfo != null)
            {
                IDataValidationInfo dataValition = (sender as FrameworkElement).DataContext as IDataValidationInfo;

                bool isSuccess = (bool)((args.OriginalSource as FrameworkElement).GetValue(Validation.HasErrorProperty));

                Binding bind = ((System.Windows.Data.BindingExpression)((System.Windows.Controls.ValidationError)args.Error).BindingInError).ParentBinding;
                if (bind != null)
                {
                    string propetyName;
                    if (bind.Source != null)
                    {
                        propetyName = bind.Path.Path + "@" + bind.Source.GetHashCode();
                    }
                    else
                    {
                        propetyName = bind.Path.Path + "@" + (sender as FrameworkElement).DataContext.GetHashCode();
                    }

                    if (isSuccess && args.Action == ValidationErrorEventAction.Added)
                    {
                        dataValition.Added(propetyName, args.Error.ErrorContent);
                    }
                    else if (!isSuccess && args.Action == ValidationErrorEventAction.Removed)
                    {
                        dataValition.Removed(propetyName);
                    }
                }
            }
        }

        /// <summary>
        /// 验证是否通过，是否包含错误
        /// </summary>
        public static readonly DependencyProperty HasErrorProperety = DependencyProperty.RegisterAttached("HasError", typeof(bool), typeof(Validations),
            new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                OnHasErrorProperetyChanged(d, e); 
                FrameworkElement fe = d as FrameworkElement;
                if (d == null)
                {
                    return;
                }
                fe.DataContextChanged -= OnHasErrorProperetyChanged2;
                fe.DataContextChanged += OnHasErrorProperetyChanged2;

            }));
        private static void OnHasErrorProperetyChanged2(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnHasErrorProperetyChanged(sender as DependencyObject, e);
        }

        private static void OnHasErrorProperetyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (d == null)
            {
                return;
            }
            if (fe.DataContext != null && fe.DataContext as IDataValidationInfo != null)
            {
                IDataValidationInfo dataValition = (d as FrameworkElement).DataContext as IDataValidationInfo;
                dataValition.ValidationChanged += (object dc, EventArgs e2) =>
                {
                    //fe.IsEnabled = !(dataValition.HasError() && Validations.GetHasError(d));
                    fe.SetValue(FrameworkElement.IsEnabledProperty, !(dataValition.HasError() && Validations.GetHasError(d)));
                };
                //fe.IsEnabled = !(dataValition.HasError() && Validations.GetHasError(d));
                fe.SetValue(FrameworkElement.IsEnabledProperty, !(dataValition.HasError() && Validations.GetHasError(d)));
            }
            else
            {
                fe.IsEnabled = !Validations.GetHasError(d);
            }
        }

        public static void SetHasError(DependencyObject obj, bool value)
        {
            obj.SetValue(HasErrorProperety, value);
        }
        public static bool GetHasError(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasErrorProperety);
        }
    }
}
