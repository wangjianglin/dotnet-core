using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Input;
using control = System.Windows.Controls;
using System.Windows.Controls;

namespace Lin.Core.Commands
{
    public enum KeyType
    {
        UP,DOWN
    }
    public class KeyToCommandBehavior : CommandBehavior<FrameworkElement>
    {
        public KeyType KeyType { get; set; }

        private void KeyEvent(object sender, KeyEventArgs e)
        {
            if (KeyType == KeyType.UP && e.IsUp)
            {
                Invoke(e);
            }
            else if (KeyType == KeyType.DOWN && e.IsDown)
            {
                Invoke(e);
            }
        }

        protected override bool CanInvoke(object parameter)
        {
            KeyEventArgs args = parameter as KeyEventArgs;
            if (args.Key != Key)    
            {
                return false;
            }
            return true;
        }

        
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.KeyDown += KeyEvent;
            this.AssociatedObject.KeyUp += KeyEvent;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.KeyDown -= KeyEvent;
            this.AssociatedObject.KeyUp -= KeyEvent;
            base.OnDetaching();
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(Key?), typeof(KeyToCommandBehavior), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
            {
                KeyToCommandBehavior command = s as KeyToCommandBehavior;
                if ((command != null) && (command.AssociatedObject != null))
                {
                    //command.EnableDisableElement();
                }
            }));
        
        public Key? Key
        {
            get { return (Key?)this.GetValue(KeyProperty); }
            set { this.SetValue(KeyProperty, value); }
        }

       

        
    }
}