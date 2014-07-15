using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Lin.Core.Utils
{
    public static class KeyBoard
    {

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CatchKeyBoardProperty = DependencyProperty.RegisterAttached("CatchKeyBoard", typeof(Key?), typeof(KeyBoard),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => 
            {
                (d as FrameworkElement).KeyDown += (object sender, KeyEventArgs e2) =>
                {
                    if (e2.Key == (Key)e.NewValue)
                    {
                        e2.Handled = true;
                    }
                };
            }));


        public static void SetCatchKeyBoard(DependencyObject obj, Key? value)
        {
            obj.SetValue(CatchKeyBoardProperty, value);
        }
        public static Key? GetCatchKeyBoard(DependencyObject obj)
        {
            return (Key?)obj.GetValue(CatchKeyBoardProperty);
        }

    }
}
