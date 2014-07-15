using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Controls; 

#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "AD.Core.Controls")]

#endregion
namespace AD.Core.Controls
{ 
    public static class FocusCommandBehavior<T>: Behavior<FrameworkElement> where T : Control
    {
        /// <summary>
        /// 判断控件是否获得焦点
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsFocused(T obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(T obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

          public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool),
              typeof(FocusCommandBehavior<T>), new PropertyMetadata((obj, e) =>
              {
                  var element = obj as T;
                  if (element != null && e.NewValue is bool && (bool)e.NewValue)
                  {
                      element.Focus();
                  }
              }));
    }
}
