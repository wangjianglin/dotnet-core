using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
 
#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "Lin.Core.Controls")]

#endregion

namespace Lin.Core.Controls
{
    /// <summary>ss
    /// 获取控件的实际宽度和高度
    /// </summary>
    public static class Controls
    {
        /// <summary>
        /// 注册ActualWidth宽度的依赖属性
        /// </summary>
        public static readonly DependencyProperty ActualWidthProperty = DependencyProperty.RegisterAttached("ActualWidth", typeof(double), typeof(Controls),
          new PropertyMetadata(0, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                FrameworkElement fe = d as FrameworkElement;

                if (fe == null)
                {
                    return;
                }

                fe.LayoutUpdated += LayoutLoadRadChart(fe);

            }));

        private static EventHandler eventHandler = null;

        public static EventHandler LayoutLoadRadChart(FrameworkElement chart)
        {
            if (eventHandler == null)
            {
                eventHandler = new EventHandler((object sender, EventArgs e3) =>
                {
                    LayoutLoadRadChartTmp(chart);
                    return;
                });
            }
            return eventHandler;
        }
        private static void LayoutLoadRadChartTmp(FrameworkElement chart)
        {
            if (chart == null)
            {
                return;
            }

            double dou1 = chart.ActualHeight;
            double dou2 = chart.ActualWidth;
        }

        public static double GetActualWidth(DependencyObject d)
        {
            return (double)d.GetValue(ActualWidthProperty);
        }
        public static void SetActualWidth(DependencyObject d, double value)
        {
            d.SetValue(ActualWidthProperty, value);
        }
        


        //注册ActualHeightProperty高度的依赖属性
        public static readonly DependencyProperty ActualHeightProperty = DependencyProperty.RegisterAttached(
            "ActualHeight", typeof(double), typeof(Controls),
          new PropertyMetadata(0, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
          {
              FrameworkElement fe = d as FrameworkElement;

              if (fe == null)
              {
                  return;
              }

          }));
        public static double GetActualHeight(DependencyObject d)
        {
            return (double)d.GetValue(ActualHeightProperty);
        }
        public static void SetActualHeight(DependencyObject d, double value)
        {
            d.SetValue(ActualHeightProperty, value);
        }
         
    }
}
