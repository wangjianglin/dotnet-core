using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Lin.Core.Commands;
using Lin.Core.Log;
using Lin.Core.Utils;
using System.Runtime.InteropServices;
using Lin.Util;
using Lin.Comm.Http;

namespace Lin.Core.Controls
{
    public static class TaskbarNotifierPopupUtil
    {
        public static void Show(object content, LogLevel level = LogLevel.INFO, string title = null)
        {
            Lin.Core.Thread.UIThread(o =>
            {
                ShowImpl(content, level, title);
            }, null);
        }
        /// <summary>
        /// 弹出提示框
        /// </summary>
        /// <param name="contentObj">异常内容</param>
        /// <param name="level">级别</param>
        /// <param name="title">标题</param>
        private static void ShowImpl(object contentObj, LogLevel level = LogLevel.INFO, string title = null)
        {
            int _level = (int)Lin.Core.Context.Global.LogLevel;
            if (contentObj == null)
            {
                return;
            }
            string code = "";
            code = GetExceptionCode(contentObj);
            string contentStr = ExceptionToString(contentObj, code);
            int l = (int)level;
            if (l < _level)
            {
                return;
            }
            if (!string.IsNullOrEmpty(contentStr))
            {
                title = title + code;
                ShowObject(contentStr, title);
            }
            else if (contentObj.GetType().Name == "String")
            {
                ShowObject(contentObj.ToString(), title);
            }
            else
            {
                ShowContext(contentObj, title);
            }
        }

        /// <summary>
        /// 传过来的对象为控件
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        private static void ShowContext(object content, string title)
        {
            // 正常提示框窗口
            PopupTaskBar objNotifier = new PopupTaskBar();
            objNotifier.Content = content;
            objNotifier.Title = title;
            objNotifier.Show();
        }

        /// <summary>
        /// 正常情况下显示提示框窗口
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        private static void ShowObject(string content, string title)
        {
            // 正常提示框窗口
            PopupTaskBar objNotifier = new PopupTaskBar();
            ScrollViewer scroll = new ScrollViewer();
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            TextBlock txt = new TextBlock();
            //txt.Background = Brushes.Red;
            txt.Width = objNotifier.Width;
            txt.Text = content;
            txt.TextWrapping = TextWrapping.WrapWithOverflow;
            scroll.Content = txt;
            objNotifier.Content = scroll;
            objNotifier.Title = title;
            objNotifier.Show();
        }

        /// <summary>
        /// 记录内部异常信息
        /// </summary>
        /// <param name="e"></param>
        /// <param name="pre"></param>
        /// <returns></returns>
        private static string ExceptionToString(Exception e, string pre)
        {
            if (e == null)
            {
                return "";
            }
            string result = pre + e.GetType().FullName + "\r\n" + e.Message + "\r\nsource:" + e.Source + "\r\ntarget site:" + e.TargetSite + "\r\nhelp link:" + e.HelpLink + "\r\nstack trace:\r\n" + e.StackTrace;
            if (e.InnerException != null)
            {
                result += ExceptionToString(e.InnerException, "\r\n\r\ninnerexception:");
            }
            return result;
        }

        /// <summary>
        /// 记录异常信息到日志文件中
        /// </summary>
        /// <param name="e"></param>
        /// <param name="pre"></param>
        /// <returns></returns>
        private static string ExceptionToString(object obj, string code)
        {
            if (obj == null)
            {
                return "";
            }
            string result = "";
            LinException e = obj as LinException;
            if (e != null)
            {
                result = ExceptionInfoToString.ADExceptionToString(obj as LinException);
                //log.Error(result);
            }
            else if (obj as Error != null)
            {
                result = ExceptionInfoToString.HttpErrorToString(obj as Error);
                //httpLog.Error(result);
            }

            return result;
        }

        /// <summary>
        /// 获取异常码的十六进制编码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetExceptionCode(object obj)
        {
            string errorInfo = "";
            long code = 0;
            LinException e = obj as LinException;
            if (e != null)
            {
                code = e.Code;
            }
            Error error = obj as Error;
            if (error != null)
            {
                code = error.code;
            }
            string errorCode = string.Format("0x{0:X}_{1:X4}", global::System.Math.Abs((long)(code / 65536)), global::System.Math.Abs(code) % 65536);
            if (code < 0)
            {
                errorInfo = "  error:-" + errorCode;
            }
            else if (code == 0)
            {
                errorInfo = "";
            }
            else
            {
                errorInfo = "error:" + errorCode;
            }

            return errorInfo;
        }
    }
    public class PopupTaskBar : DependencyObject
    {
        private PopupWindow popup = null;
        private double horizontalOffset = 0;
        private double verticalOffset = 0;
        private PopupTaskbarContent notifierContent = new PopupTaskbarContent();

        #region 属性
        /// <summary>
        /// 提示框要显示的标题
        /// </summary>
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(PopupTaskBar),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                PopupTaskBar t = sender as PopupTaskBar;
                t.notifierContent.Title = e.NewValue;
            }));
        public object Title
        {
            get { return this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 提示框要显示的内容
        /// </summary>
        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(PopupTaskBar),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                PopupTaskBar t = sender as PopupTaskBar;
                t.notifierContent.Content = e.NewValue;
            }));
        public object Content
        {
            get { return this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 提示框要显示的位置
        /// </summary>
        public readonly static DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(TaskbarNotifierPosition), typeof(PopupTaskBar),
            new PropertyMetadata(TaskbarNotifierPosition.RightTop, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                PopupTaskBar taskbar = (sender as PopupTaskBar);
                //taskbar.SetLocation();
            }));

        public TaskbarNotifierPosition Position
        {
            get { return (TaskbarNotifierPosition)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// 提示框要显示的高
        /// </summary>
        public readonly static DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(PopupTaskBar),
            new PropertyMetadata(160.0, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {

            }));
        public double Height
        {
            get { return (double)this.GetValue(HeightProperty); }
            set { this.SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// 提示框要显示的宽
        /// </summary>
        public readonly static DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(PopupTaskBar),
            new PropertyMetadata(300.0, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {

            }));
        public double Width
        {
            get { return (double)this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// 提示框显示时间(毫秒为单位)
        /// </summary>
        public readonly static DependencyProperty OpenTimeProperty = DependencyProperty.Register("OpenTime", typeof(int), typeof(PopupTaskBar),
            new PropertyMetadata(0, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {

            }));
        /// <summary>
        /// 提示框显示时间(毫秒为单位)
        /// </summary>
        public int OpenTime
        {
            get { return (int)this.GetValue(OpenTimeProperty); }
            set { this.SetValue(OpenTimeProperty, value); }
        }

        /// <summary>
        /// 提示框自动消失所用时间(毫秒为单位)
        /// </summary>
        public readonly static DependencyProperty HiddingTimeProperty = DependencyProperty.Register("HiddingTime", typeof(int), typeof(PopupTaskBar),
            new PropertyMetadata(1000, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {

            }));
        /// <summary>
        /// 提示框自动消失所用时间(毫秒为单位)
        /// </summary>
        public int HiddingTime
        {
            get { return (int)this.GetValue(HiddingTimeProperty); }
            set { this.SetValue(HiddingTimeProperty, value); }
        }

        /// <summary>
        /// 弹出提示框所需的时间(毫秒为单位)
        /// </summary>
        public readonly static DependencyProperty OpeningTimeProperty = DependencyProperty.Register("OpeningTime", typeof(int), typeof(PopupTaskBar),
            new PropertyMetadata(1000, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {

            }));
        /// <summary>
        /// 弹出提示框所需的时间(毫秒为单位)
        /// </summary>
        public int OpeningTime
        {
            get { return (int)this.GetValue(OpeningTimeProperty); }
            set { this.SetValue(OpeningTimeProperty, value); }
        }

        /// <summary>
        /// 是否显示动画效果
        /// </summary>
        public readonly static DependencyProperty IsAnimationProperty = DependencyProperty.Register("IsAnimation", typeof(bool), typeof(PopupTaskBar),
            new PropertyMetadata(false, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {

            }));
        /// <summary>
        /// 是否显示动画效果
        /// </summary>
        public bool IsAnimation
        {
            get { return (bool)this.GetValue(IsAnimationProperty); }
            set { this.SetValue(IsAnimationProperty, value); }
        }
        #endregion

        public PopupTaskBar()
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
            rect = System.Windows.Forms.Screen.GetWorkingArea(rect);
            if (Position == TaskbarNotifierPosition.LeftCorner)
            {
                horizontalOffset = 0;
                verticalOffset = rect.Height - this.Height;
            }
            else if (Position == TaskbarNotifierPosition.LeftTop)
            {
                horizontalOffset = 0;
                verticalOffset = 0;
            }
            else if (Position == TaskbarNotifierPosition.RightCorner)
            {
                horizontalOffset = rect.Width - this.Width;
                verticalOffset = rect.Height - this.Height;
            }
            else if (Position == TaskbarNotifierPosition.RightTop)
            {
                horizontalOffset = rect.Width - this.Width;
                verticalOffset = 0;
            }
            popup = new PopupWindow(this);
            popup.HorizontalOffset = horizontalOffset;
            popup.VerticalOffset = verticalOffset;
        }

        /// <summary>
        /// 弹出提示框窗口
        /// </summary>
        public void Show()
        {
            popup.Width = Width;
            popup.Height = Height;
            popup.Name = "popupWindow";
            popup.HorizontalAlignment = HorizontalAlignment.Right;
            popup.VerticalAlignment = VerticalAlignment.Top;
            popup.IsOpen = true;
            popup.Placement = PlacementMode.Relative;
            popup.PopupAnimation = PopupAnimation.Fade;
            popup.AllowsTransparency = true;
            popup.Child = notifierContent;
            popup.IsOpen = true;
            (popup as PopupWindow).PopupTaskBar_Loaded();
        }

        private class PopupWindow : Popup
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }
            }
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool GetCursorPos(out POINT pt);

            private Point mousePoint;
            PopupTaskBar taskbarNotifier = null;
            private DispatcherTimer stayOpenTimer = null;
            private Storyboard storyboard;
            private DoubleAnimation animation;

            private double hiddenTop;
            private double openedTop;
            private EventHandler arrivedHidden;
            private EventHandler arrivedOpened;

            private int openingMilliseconds = 0;
            private int hidingMilliseconds = 0;
            private int stayOpenMilliseconds = 0;
            private bool isAnimation = true;
            private TaskbarNotifierPosition position = 0;
            public PopupWindow(PopupTaskBar popup)
            {
                this.stayOpenMilliseconds = popup.OpenTime;
                this.openingMilliseconds = popup.OpeningTime;
                this.hidingMilliseconds = popup.HiddingTime;
                this.isAnimation = popup.IsAnimation;
                this.position = popup.Position;
                this.AllowDrop = true;
                this.MouseLeftButtonUp += PopupWindow_MouseLeftButtonUp;

                this.MouseMove += PopupWindow_MouseMove;
                this.MouseDown += PopupWindow_MouseDown;
                this.MouseUp += PopupWindow_MouseUp;
                this.MouseMove += PopupWindow_MouseMove;
                taskbarNotifier = popup;
            }
            protected override void OnMouseDown(MouseButtonEventArgs e)
            {
                base.OnMouseDown(e);
            }

            void PopupWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                isLeftButtonPressed = false;
            }

            bool isLeftButtonPressed;
            bool isMouseCaptured;
            double mouseVerticalPosition;
            double mouseHorizontalPosition;
            void PopupWindow_MouseUp(object sende, MouseButtonEventArgs e)
            {
                isMouseCaptured = false;
                this.ReleaseMouseCapture();
                mouseVerticalPosition = -1;
                mouseHorizontalPosition = -1;
            }

            void PopupWindow_MouseDown(object sender, MouseButtonEventArgs e)
            {
                mouseVerticalPosition = e.GetPosition(null).Y;
                mouseHorizontalPosition = e.GetPosition(null).X;
                isMouseCaptured = true;
                this.CaptureMouse();
            }

            void PopupWindow_MouseMove(object sender, MouseEventArgs args)
            {
                POINT pt = new POINT();
                GetCursorPos(out pt);
                if (isMouseCaptured)
                {
                    double deltaV = args.GetPosition(null).Y - mouseVerticalPosition;
                    double deltaH = args.GetPosition(null).X - mouseHorizontalPosition;
                    double newTop = deltaV + pt.Y;
                    double newLeft = deltaH + pt.X;

                    this.SetValue(Popup.VerticalOffsetProperty, newTop);
                    this.SetValue(Popup.HorizontalOffsetProperty, newLeft);

                    mouseVerticalPosition = args.GetPosition(null).Y;
                    mouseHorizontalPosition = args.GetPosition(null).X;
                }
            }

            /// <summary>
            /// 显示的状态
            /// </summary>
            private enum DisplayStates
            {
                Opening,
                Opened,
                Hiding,
                Hidden
            }

            private DisplayStates displayState;
            /// <summary>
            /// 当前的显示状态
            /// </summary>
            private DisplayStates DisplayState
            {
                get
                {
                    return this.displayState;
                }
                set
                {
                    if (value != this.displayState)
                    {
                        this.displayState = value;
                        this.OnDisplayStateChanged();
                    }
                }
            }

            static PopupWindow()
            {
                CommandManager.RegisterClassCommandBinding(typeof(Popup), new CommandBinding(RoutedCommands.Commands["TaskbarNotifierWindowColose"],
                 (object sender, ExecutedRoutedEventArgs e) =>
                 {
                     PopupWindow notifierWindow = (sender as PopupWindow);
                     notifierWindow.ForceHidden();
                 }));
            }

            /// <summary>
            /// 窗体加载事件(动画效果弹出窗口)
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            public void PopupTaskBar_Loaded()
            {
                SetInitialLocations(false);
                this.DisplayState = DisplayStates.Hidden;

                // 准备时间器，默认显示多长时间
                this.stayOpenTimer = new DispatcherTimer();
                this.stayOpenTimer.Interval = TimeSpan.FromMilliseconds(this.stayOpenMilliseconds);
                this.stayOpenTimer.Tick += new EventHandler(this.stayOpenTimer_Elapsed);

                // 动画来改变顶部的属性
                this.animation = new DoubleAnimation();
                Storyboard.SetTargetProperty(this.animation, new PropertyPath(Popup.VerticalOffsetProperty));
                this.storyboard = new Storyboard();
                this.storyboard.Children.Add(this.animation);
                this.storyboard.FillBehavior = FillBehavior.Stop;
                // 当动画完成时，激活事件，显示或者隐藏
                this.arrivedHidden = new EventHandler(this.Storyboard_ArrivedHidden);
                this.arrivedOpened = new EventHandler(this.Storyboard_ArrivedOpened);
                Notify();
            }

            /// <summary>
            /// 当显示状态发生改变时
            /// </summary>
            private void OnDisplayStateChanged()
            {
                this.stayOpenMilliseconds = taskbarNotifier.OpenTime;
                this.openingMilliseconds = taskbarNotifier.OpeningTime;
                this.hidingMilliseconds = taskbarNotifier.HiddingTime;
                this.isAnimation = taskbarNotifier.IsAnimation;
                this.position = taskbarNotifier.Position;
                if (this.storyboard == null)
                    return;

                this.storyboard.Stop(this); // 停止动画效果

                //由于用于打开和关闭，都可能被重用故事板,已完成的事件处理程序需要被删除
                this.storyboard.Completed -= arrivedHidden;
                this.storyboard.Completed -= arrivedOpened;

                // 处于已经打开状态时
                if (this.displayState == DisplayStates.Opened)
                {
                    this.SetInitialLocations(true);
                    if (!this.IsMouseOver)
                    {
                        // 鼠标不在窗口的范围里面时，窗口显示设定的时间后，隐藏。当鼠标在窗口的范围里面，窗口将一直显示到屏幕上面
                        this.stayOpenTimer.Stop();
                        this.stayOpenTimer.Start();
                    }
                }
                else if (this.displayState == DisplayStates.Opening) // 窗口正在打开
                {
                    this.Visibility = Visibility.Visible;
                    // 当有部分窗口已经打开，且该速度只是一小部分的正常速度，那得重新计算未打开窗口的速度
                    int milliseconds = this.CalculateMillseconds(this.openingMilliseconds, this.openedTop);
                    if (milliseconds < 1)
                    {
                        // 窗口必须处于已经打开的状态
                        this.DisplayState = DisplayStates.Opened;
                        return;
                    }

                    if (isAnimation)
                    {
                        // 重新设置动画效果
                        this.animation.To = this.openedTop;
                        this.animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, milliseconds));
                    }
                    this.storyboard.Completed += arrivedOpened; // 完成后事件处理
                    this.storyboard.Begin(this, true); // 开始显示动画效果
                }
                else if (this.displayState == DisplayStates.Hiding) // 正在慢慢的隐藏窗口
                {
                    if (this.stayOpenMilliseconds <= 0)
                    {
                        this.displayState = DisplayStates.Opened;
                        return;
                    }
                    // 当有部分窗口已经隐藏，且该速度只是一小部分的正常速度，那得重新计算未隐藏窗口的速度
                    int milliseconds = this.CalculateMillseconds(this.hidingMilliseconds, this.hiddenTop);

                    if (milliseconds < 1)
                    {
                        this.DisplayState = DisplayStates.Hidden;// 已经隐藏
                        return;
                    }
                    if (isAnimation)
                    {
                        this.animation.To = this.hiddenTop;
                        this.animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, milliseconds));
                    }
                    this.storyboard.Completed += arrivedHidden;
                    this.storyboard.Begin(this, true);
                }
                else if (this.displayState == DisplayStates.Hidden)
                {
                    Close();
                }
            }

            /// <summary>
            /// 根据到达目的需要花费的时间来计算还需要花多长时间
            /// </summary>
            /// <param name="totalMillsecondsNormally">设置的总共要花的时间长</param>
            /// <param name="destination">目的地</param>
            /// <returns>还需花的时间</returns>
            private int CalculateMillseconds(int totalMillsecondsNormally, double destination)
            {
                if (this.VerticalOffset == destination)
                {
                    return 0;
                }
                double distanceRemaining = global::System.Math.Abs(this.VerticalOffset - destination);
                double percentDone = distanceRemaining / this.Height;
                // 确定实际需要的正常（以毫秒为单位）的百分比的。
                return (int)(totalMillsecondsNormally * percentDone);
            }

            /// <summary>
            /// 当时间线已经到达隐藏，窗口的状态为隐藏
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected virtual void Storyboard_ArrivedHidden(object sender, EventArgs e)
            {
                this.DisplayState = DisplayStates.Hidden;
            }

            /// <summary>
            /// 当时间线已经到达打开，窗口的状态为已经打开
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected virtual void Storyboard_ArrivedOpened(object sender, EventArgs e)
            {
                this.DisplayState = DisplayStates.Opened;
            }

            /// <summary>
            /// 后台线程消失时，触发的事件
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            private void stayOpenTimer_Elapsed(Object sender, EventArgs args)
            {
                this.stayOpenTimer.Stop(); // 停止计时器
                if (!this.IsMouseOver)
                {
                    this.DisplayState = DisplayStates.Hiding; // 鼠标不再窗口范围里，隐藏
                }
            }

            /// <summary>
            /// 鼠标进入窗口的区域里触发的事件
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
            {
                if (this.DisplayState == DisplayStates.Opened)
                {
                    // 当窗口已经打开时，停止计时器，或者窗口会隐藏
                    this.stayOpenTimer.Stop();
                }
                else if ((this.DisplayState == DisplayStates.Hidden) ||
                         (this.DisplayState == DisplayStates.Hiding))
                {
                    // 当鼠标进入窗口区域，而此时窗口隐藏或者正在被隐藏。应打开窗口
                    this.DisplayState = DisplayStates.Opening;
                }
                base.OnMouseEnter(e);
            }

            /// <summary>
            /// 鼠标离开窗口的区域触发的事件
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
            {
                if (this.DisplayState == DisplayStates.Opened)
                {
                    // 启动定时器，定时隐藏窗口
                    this.stayOpenTimer.Stop();
                    this.stayOpenTimer.Start();
                }

                base.OnMouseEnter(e);
            }

            /// <summary>
            /// 窗口已经打开，并应继续启动计时器重新计算时间来控制窗口的显示状态
            /// </summary>
            public void Notify()
            {
                if (this.DisplayState == DisplayStates.Opened)
                {
                    this.stayOpenTimer.Stop();
                    this.stayOpenTimer.Start();
                }
                else
                {
                    this.DisplayState = DisplayStates.Opening;
                }
            }

            /// <summary>
            /// 强制隐藏窗口
            /// </summary>
            public void ForceHidden()
            {
                this.DisplayState = DisplayStates.Hidden;
            }

            #region 属性改变事件

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion

            /// <summary>
            /// 设置提示框初始值的位置
            /// </summary>
            /// <param name="showOpened">是否打开</param>
            private void SetInitialLocations(bool showOpened)
            {
                // 获取屏幕的工作区域
                System.Drawing.Rectangle workingArea = new System.Drawing.Rectangle((int)this.HorizontalOffset, (int)this.VerticalOffset, (int)this.Width, (int)this.Height);
                workingArea = System.Windows.Forms.Screen.GetWorkingArea(workingArea);
                switch (position)
                {
                    case TaskbarNotifierPosition.LeftCorner:
                        this.HorizontalOffset = 0;
                        this.openedTop = workingArea.Bottom - this.Height;
                        this.hiddenTop = workingArea.Bottom;
                        break;
                    case TaskbarNotifierPosition.LeftTop:
                        this.HorizontalOffset = 0;
                        this.openedTop = 0;
                        this.hiddenTop = workingArea.Top - this.Height;
                        break;
                    case TaskbarNotifierPosition.RightCorner:
                        // 将窗口开始的位置默认为右下角
                        this.HorizontalOffset = workingArea.Right - this.Width;
                        this.openedTop = workingArea.Bottom - this.Height;
                        this.hiddenTop = workingArea.Bottom;
                        break;
                    case TaskbarNotifierPosition.RightTop:
                        this.HorizontalOffset = workingArea.Right - this.Width;
                        this.openedTop = 0;
                        this.hiddenTop = workingArea.Top - this.Height;
                        break;
                }

                // 根据窗口是否打开或者隐藏来设置窗口顶部的位置
                if (showOpened)
                    this.VerticalOffset = openedTop;
                else
                    this.VerticalOffset = hiddenTop;
            }

            /// <summary>
            /// 动画关闭 
            /// </summary>
            public new void Close()
            {
                ScaleTransform st = new ScaleTransform(1, 1);
                DoubleAnimation cartoonClear = new DoubleAnimation(1, 0.5, new Duration(TimeSpan.FromMilliseconds(90)));
                this.RenderTransform = st;
                cartoonClear.Completed += (c, com) =>
                {
                    this.IsOpen = false;
                };
                st.BeginAnimation(ScaleTransform.ScaleYProperty, cartoonClear);
                st.BeginAnimation(ScaleTransform.ScaleXProperty, cartoonClear);
            }
        }
    }
}
