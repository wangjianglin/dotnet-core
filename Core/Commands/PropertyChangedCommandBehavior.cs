using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Lin.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyChangedCommandBehavior : CommandBehavior<FrameworkElement>
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(KeyToCommandBehavior), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            //KeyToCommandBehavior command = s as KeyToCommandBehavior;
            //if ((command != null) && (command.AssociatedObject != null))
            //{
            //    //command.EnableDisableElement();
            //}
        }));


        protected override void OnAttached()
        {
            base.OnAttached();
            TextBox fe = this.AssociatedObject as TextBox;
            //TextBox.TextProperty.\
            
        }

        protected override void OnDetaching()
        {
            //this.AssociatedObject.KeyDown -= KeyEvent;
            //this.AssociatedObject.KeyUp -= KeyEvent;
            //base.OnDetaching();
        }
    }
}
