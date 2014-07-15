using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lin.Core.Controls
{
    public class PopupTaskbarContent : Control
    {
        static PopupTaskbarContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupTaskbarContent), new FrameworkPropertyMetadata(typeof(PopupTaskbarContent)));
        }
        /// <summary>
        /// 提示框要显示的标题
        /// </summary>
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(PopupTaskbarContent),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {

            }));
        public object Title
        {
            get { return this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 提示框要显示的内容
        /// </summary>
        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(PopupTaskbarContent),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                
            }));
        public object Content
        {
            get { return this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }
    }
}
