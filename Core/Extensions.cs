using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Lin.Core
{
    public static class Extensions
    {
        /// <summary>
        /// 设置FrameworkElement的子元素的DataContext，但不设FrameworkElement的DataContext，
        /// 主要为了防止FrameworkElement的DataContext被覆盖而引入bug
        /// </summary>
        /// <param name="element"></param>
        /// <param name="context"></param>
        public static void SetDateContext(this FrameworkElement element, object context)
        {
            System.Action set = ()=>{
                int N = VisualTreeHelper.GetChildrenCount(element);
                    FrameworkElement fe = null;
                    for (int n = 0; n < N; n++)
                    {
                        fe = VisualTreeHelper.GetChild(element, n) as FrameworkElement;
                        if (fe != null)
                        {
                            fe.DataContext = context;
                        }
                    }
            };
            if (element.IsLoaded)
            {
                set();
            }
            else
            {
                EventHandler render = null;
                EventHandler renderimpl = (object sender, EventArgs e) =>
                 {
                     element.LayoutUpdated -= render;
                     set();
                 };
                render = renderimpl;
                element.LayoutUpdated += render;
                //RoutedEventHandler render = null;
                //RoutedEventHandler renderimpl = (object sender, RoutedEventArgs e) =>
                //{
                //    element.Loaded -= render;
                //    set();
                //};
                //render = renderimpl;
                //element.Loaded += render;
            }
            
        }

    }
}
