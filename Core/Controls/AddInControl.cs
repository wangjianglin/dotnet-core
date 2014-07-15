using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lin.Core.AddIn;
using System.Windows;

namespace Lin.Core.Controls
{
    /// <summary>
    /// 根据AddIn加载内容，加载插件Type为AddInType的插件，如果没有设置版本号，则加载版本号最大的插件，否则加载指定版本号的插件
    /// 
    /// 2012-09-17 
    /// 王江林
    /// </summary>
    public class AddInControl : Lin.Core.Controls.View
    {
        static AddInControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AddInControl), new FrameworkPropertyMetadata(typeof(AddInControl)));
        }
        public static readonly DependencyProperty AddInTypeProperty = DependencyProperty.Register("AddInType", typeof(string), typeof(AddInControl),
            new PropertyMetadata(null, (DependencyObject dc, DependencyPropertyChangedEventArgs args) =>
            {
                ((AddInControl)dc).RefreshContent();
            }));
        /// <summary>
        /// 插件名称
        /// </summary>
        public string AddInType
        {
            get { return (string)this.GetValue(AddInTypeProperty); }
            set { this.SetValue(AddInTypeProperty, value); }
        }

        public static readonly DependencyProperty MajorProperty = DependencyProperty.Register("Major", typeof(int), typeof(AddInControl),
            new PropertyMetadata(-1, (DependencyObject dc, DependencyPropertyChangedEventArgs args) =>
            {
                ((AddInControl)dc).RefreshContent();
            }));
        /// <summary>
        /// 主版本号
        /// </summary>
        public int Major
        {
            get { return (int)this.GetValue(MajorProperty); }
            set { this.SetValue(MajorProperty, value); }
        }

        public static readonly DependencyProperty MinorProperty = DependencyProperty.Register("Minor", typeof(int), typeof(AddInControl),
            new PropertyMetadata(-1, (DependencyObject dc, DependencyPropertyChangedEventArgs args) =>
            {
                ((AddInControl)dc).RefreshContent();
            }));
        /// <summary>
        /// 次版本号
        /// </summary>
        public int Minor
        {
            get { return (int)this.GetValue(MinorProperty); }
            set { this.SetValue(MinorProperty, value); }
        }

        public AddInControl()
        {
            //this.Major = -1;
            //this.Minor = -1;
        }
        private void RefreshContent()
        {
            if (isApplyTemplate == false)
            {
                return;
            }
            AddInToken current = null;
            foreach (AddInToken token in base.AddIns)
            {
                //判断Type是否相同
                if (token.Type == AddInType)
                {
                    //判断 主版本号 是否为指定版本号，如果是，则要相等，否则不满足要求
                    if (this.Major >= 0 && this.Major != token.Major)
                    {
                        continue;
                    }

                    //判断 次版本号 是否为指定版本号，如果是，则要相等，否则不满足要求
                    if (this.Minor >= 0 && this.Minor != token.Minor)
                    {
                        continue;
                    }

                    //执行到这里，说明要么取版本号大的，要么为指定版本号
                    if (current == null)//如果current为null，表明是第一个满足要求的，也就是已经满足要求的版本号最大的
                    {
                        current = token;
                        continue;
                    }
                    //如果 主版本号  是取最大的则进行判断，
                    if (this.Major < 0)// token.Major > current.Major)
                    {
                        if (token.Major > current.Major)
                        {
                            current = token;
                            continue;
                        }
                        else if (token.Major < current.Minor)//如果 主版本号 比当前英的主版本号还小，就不用对 次版本进行判断
                        {
                            break;
                        }
                    }
                    //判断 次版本号
                    if (this.Minor < 0 && token.Minor > current.Minor)
                    {
                        current = token;
                        continue;
                    }
                }
            }
            if (current != null)
            {
                this.Content = current.Content;
            }
        }
        /// <summary>
        /// 记录当前控件是否已经渲染，只渲染后才加载插件的内容
        /// </summary>
        private bool isApplyTemplate = false;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            isApplyTemplate = true;
            RefreshContent();
        }
    }
}
