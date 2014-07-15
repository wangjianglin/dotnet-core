using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Lin.Core.Controls
{
    /*
     *作者:刘鑫
     *时间:2012-11-08
     *说明：工具类（设置页面的DataContext）
     */
    public class AdControl : System.Windows.Controls.Control
    {
        /// <summary>
        /// 设置DataContext
        /// </summary>
        /// <param name="dataContext"></param>
        protected void SetDataContext(object dataContext)
        {
            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                FrameworkElement f1 = this.GetVisualChild(i) as FrameworkElement;
                if (f1 != null)
                {
                    f1.DataContext = dataContext;
                }
                else
                {
                    this.DataContext = dataContext;
                }
            }
        }
    }
}
