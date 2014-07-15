using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lin.Core.Controls
{
    /// <summary>
    /// MetorBusyContor.xaml 的交互逻辑
    /// </summary>
    public partial class MetorBusyContor : UserControl, INotifyPropertyChanged
    {
        public MetorBusyContor()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (double.IsNaN(Width))//默认为400的宽度
            {
                Width = 200;
            }
            LeftFrom = 0;
            LeftTo = Width / 2 - (Width / 7) / 2;
            SlowFrom = LeftTo;
            SlowTo = LeftTo + (Width / 7);
            RightFrom = SlowTo;
            RightTo = Width;

            Start();
        }

        #region 属性
        private double _leftFrom;
        /// <summary>
        /// 左边第一个起点
        /// </summary>
        public double LeftFrom
        {
            get { return _leftFrom; }
            set
            {
                _leftFrom = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("LeftFrom");
                }
            }
        }

        private double _leftTo;
        /// <summary>
        /// 第一个终点
        /// </summary>
        public double LeftTo
        {
            get
            {
                return _leftTo;
            }
            set
            {
                _leftTo = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("LeftTo");
                }
            }
        }

        private double _slowFrom;
        /// <summary>
        /// 缓动起点
        /// </summary>
        public double SlowFrom
        {
            get
            {
                return _slowFrom;
            }
            set
            {
                _slowFrom = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("SlowFrom");
                }
            }
        }

        private double _slowTo;
        /// <summary>
        /// 缓动终点
        /// </summary>
        public double SlowTo
        {
            get
            {
                return _slowTo;
            }
            set
            {
                _slowTo = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("SlowTo");
                }
            }
        }

        private double _rightFrom;
        /// <summary>
        /// 右边起点
        /// </summary>
        public double RightFrom
        {
            get
            {
                return _rightFrom;
            }
            set
            {
                _rightFrom = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("RightFrom");
                }
            }
        }

        private double _rightTo;
        /// <summary>
        /// 右边终点
        /// </summary>
        public double RightTo
        {
            get
            {
                return _rightTo;
            }
            set
            {
                _rightTo = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("RightTo");
                }
            }
        }
        #endregion

        private void CtlWin8ProcessBar_OnInitialized(object sender, EventArgs e)
        {
            this.DataContext = this;

            this.el.Opacity = 0;
            this.el1.Opacity = 0;
            this.el2.Opacity = 0;
            this.el3.Opacity = 0;

            var sbLeft = this.el.FindResource("sbLeft") as Storyboard;
            var sbSlow = this.el.FindResource("sbSlow") as Storyboard;
            var sbRight = this.el.FindResource("sbRight") as Storyboard;

            var sbLeft1 = this.el1.FindResource("sbLeft1") as Storyboard;
            var sbSlow1 = this.el1.FindResource("sbSlow1") as Storyboard;
            var sbRight1 = this.el1.FindResource("sbRight1") as Storyboard;

            var sbLeft2 = this.el2.FindResource("sbLeft2") as Storyboard;
            var sbSlow2 = this.el2.FindResource("sbSlow2") as Storyboard;
            var sbRight2 = this.el2.FindResource("sbRight2") as Storyboard;

            var sbLeft3 = this.el3.FindResource("sbLeft3") as Storyboard;
            var sbSlow3 = this.el3.FindResource("sbSlow3") as Storyboard;
            var sbRight3 = this.el3.FindResource("sbRight3") as Storyboard;

            //第一个点第一个动画结束后开启缓动，第二个点启动
            sbLeft.Completed += (a, b) =>
            {
                sbSlow.Begin();
                el1.Opacity = 1;
                sbLeft1.Begin();
            };
            //第一个点缓动结束，右边动画启动
            sbSlow.Completed += (a, b) => sbRight.Begin();
            sbRight.Completed += (a, b) => el.Opacity = 0;
            //以下类推
            sbLeft1.Completed += (a, b) =>
            {
                sbSlow1.Begin();
                el2.Opacity = 1;
                sbLeft2.Begin();
            };
            sbSlow1.Completed += (a, b) => sbRight1.Begin();
            sbRight1.Completed += (a, b) => el1.Opacity = 0;

            sbLeft2.Completed += (a, b) =>
            {
                sbSlow2.Begin();
                el3.Opacity = 1;
                sbLeft3.Begin();
            };
            sbSlow2.Completed += (a, b) => sbRight2.Begin();
            sbRight2.Completed += (a, b) => el2.Opacity = 0;

            sbLeft3.Completed += (a, b) => sbSlow3.Begin();
            sbSlow3.Completed += (a, b) => sbRight3.Begin();
            //最后一个点动画结束，第一个点重启 如此循环
            sbRight3.Completed += (a, b) =>
            {
                el3.Opacity = 0;
                el.Opacity = 1;
                sbLeft.Begin();
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Start()
        {
            var sb = this.el.FindResource("sbLeft") as Storyboard;
            this.el.Opacity = 1;
            if (sb != null)
                sb.Begin();
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
