using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace Lin.Core.Controls
{
    /// <summary>
    /// 状态管理，通过绑定改变控件的状态
    /// </summary>
    public static class StateManager
    {
        private static void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null)
            {
                System.Windows.VisualStateManager.GoToState(fe, GetStateValue(fe), true);
                fe.SizeChanged -= SizeChanged;
            }
        }

        private static void IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null)
            {
                System.Windows.VisualStateManager.GoToState(fe, GetStateValue(fe), true);
            }
        }
        /// <summary>
        /// 控件状态
        /// </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached("State", typeof(string), typeof(StateManager),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                string state = e.NewValue as string;
                if (state != null)
                {
                    FrameworkElement fe = d as FrameworkElement;
                    if (fe != null)
                    {
                        if (GetStateValue(fe) == null)
                        {
                            fe.SizeChanged -= SizeChanged;
                            fe.SizeChanged += SizeChanged;
                        }
                        SetStateValue(fe, state);
                        System.Windows.VisualStateManager.GoToState(fe, state, true);
                    }
                }
            }));

        public static string GetState(DependencyObject obj)
        {
            return (string)obj.GetValue(StateProperty);
        }

        public static void SetState(DependencyObject obj, string value)
        {
            obj.SetValue(StateProperty, value);
        }


        private static readonly DependencyProperty StateValueProperty = DependencyProperty.RegisterAttached("StateValue", typeof(string), typeof(StateManager),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                if (d is FrameworkElement)
                {
                    System.Windows.VisualStateManager.GoToState(d as FrameworkElement, e.NewValue as string, false);
                }
            }));

        private static void SetStateValue(DependencyObject d, string value)
        {
            d.SetValue(StateValueProperty, value);
        }

        private static string GetStateValue(DependencyObject d)
        {
            return d.GetValue(StateValueProperty) as string;
        }
    }
}
