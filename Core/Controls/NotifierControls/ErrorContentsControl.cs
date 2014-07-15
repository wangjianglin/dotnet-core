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

namespace Lin.Core.Controls.NotifierControls
{
    public class ErrorContentsControl : Control
    {
        static ErrorContentsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ErrorContentsControl), new FrameworkPropertyMetadata(typeof(ErrorContentsControl)));
        }

        /// <summary>
        /// 组装成ErrorContent对象列表
        /// </summary>
        public readonly static DependencyProperty ErrorContentsProperty = DependencyProperty.Register("ErrorContents", typeof(IList<string>), typeof(ErrorContentsControl),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
            }));

        public IList<string> ErrorContents
        {
            set { this.SetValue(ErrorContentsProperty, value); }
            get { return (IList<string>)this.GetValue(ErrorContentsProperty); }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemsControl tc = this.GetTemplateChild("ErrorItems") as ItemsControl;
            if (tc != null)
            {
                TextBlock tb = new TextBlock();
                foreach (string str in ErrorContents)
                {
                    tb.Inlines.Add(new Run(str));
                    tb.Inlines.Add(new LineBreak());
                }
                tc.Items.Add(tb);
            }
        }
    }
}
