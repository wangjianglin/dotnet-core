using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Interactivity;



namespace Lin.Core.Controls
{
    /// <summary>
    /// 此控件用于文本框获得焦点事件
    /// </summary>
    public class ControlFocus
    {
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
            "IsFocused", typeof(bool), typeof(ControlFocus),
            new PropertyMetadata(IsFocusedPropertyChanged));

        private static void IsFocusedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Control p = dependencyObject as Control;
            if (p != null && Convert.ToBoolean(e.NewValue) == true)
            {
                p.Focus();
            }
        }
        public static bool GetIsFocused(DependencyObject p)
        {
            return p.GetValue(IsFocusedProperty) is bool ? (bool)p.GetValue(IsFocusedProperty) : false;
        }

        public static void SetIsFocused(DependencyObject p, bool value)
        {
            p.SetValue(IsFocusedProperty, value);
        }
    }
}
