using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Lin.Core.Commands;
using Lin.Core.Utils;
using Lin.Core.Web.Http;
using WindowForm = System.Windows.Forms;
using LogInfo = Lin.Core.Log;
using Lin.Core.Log;
using Lin.Core.Controls.NotifierControls;
using Lin.Util;

namespace Lin.Core.Controls
{
    /// <summary>
    /// 显示的状态
    /// </summary>
    public enum DisplayStates
    {
        Opening,
        Opened,
        Hiding,
        Hidden
    }

    public static class TaskbarNotifierUtil
    {
        internal static Dictionary<long, TaskbarNotifier> dict = new Dictionary<long, TaskbarNotifier>();
        private static LogInfo.ILogger log = LogInfo.Logger.Logs["AD.Exception"];
        private static LogInfo.ILogger httpLog = LogInfo.Logger.Logs["Error.Exception"];
        internal static long id = 0;


        private static void Show2(object content, LogLevel level = LogLevel.INFO, string title = null)
        {
            Lin.Core.Utils.Thread.UIThread(o =>
            {
                ShowImpl(content, level, title);
            }, null);
        }

        /// <summary>
        /// 根据提示框的内容不同显示相应的控件
        /// </summary>
        /// <param name="content">要显示的内容</param>
        /// <param name="level">级别</param>
        /// <param name="title">标题</param>
        public static void Show(object content, LogLevel level = LogLevel.INFO, string title = null)
        {
            Lin.Core.Utils.Thread.BackThread(obj =>
            {
                ShowBackThread(content, level, title);
            });
        }

        public static void ShowBackThread(object content, LogLevel level = LogLevel.INFO, string title = null)
        {
            System.Threading.AutoResetEvent auto = new System.Threading.AutoResetEvent(false);
            bool isAccord = true;
            Lin.Core.Utils.Thread.UIThread(obj => 
            {
                if (System.Windows.Application.Current.MainWindow == null)
                {
                    isAccord = false;                    
                }
                auto.Set();
            });
            auto.WaitOne();
            if (!isAccord)
            {
                return;
            }
            //if (Lin.Plugin.ApplicationLife.ApplicationLifecycleManager.Current.Phase < Lin.Plugin.ApplicationLife.ApplicationLifecyclePhase.ENTER_APP)
            //{
            //    return;
            //}
            int _level = (int)Lin.Core.ViewModel.Context.Global.LogLevel;
            int l = (int)level;

            Type type = content.GetType();

            #region 根据唯一标识ID是否已弹出框提示框

            TaskbarNotifier win = null;
            try
            {
                id = Convert.ToInt64(type.GetProperty("Id").GetValue(content, null));
                if (dict.ContainsKey(id)) // 集合是否存在指定的Key(存在：窗口已经打开)
                {
                    win = dict[id];
                }
            }
            catch { }

            if (win != null && id != 0)
            {
                win.isFirst = false;
                win.ShowSingleWindow(win);
                return;
            }

            #endregion

            if (title == null)
            {
                try
                {
                    title = type.GetProperty("Title").GetValue(content, null).ToString();
                }
                catch
                {
                    title = "温馨提示";
                }
            }

            #region 后台异常

            Lin.Core.Web.Http.Error error = null;
            try
            {
                error = type.GetProperty("Error").GetValue(content, null) as Error;
            }
            catch
            { }
            if (error == null)
            {
                error = content as Error;
            }
            if (error != null)
            {
                httpLog.Error(ExceptionInfoToString.HttpErrorToString(error));
                if (l < _level)
                {
                    return;
                }
                if (error.code == 0x2000001 || error.code == 0x2000003) // 与服务器断开连接
                {
                    Lin.Core.Utils.Thread.UIThread(o =>
                   {
                       ShowContext(new NetWorkExceptionControl(), title);
                   });
                }
                else if (error.code / 0x10000 == -200)//非网络问题，并且是前台的
                {
                    Lin.Core.Utils.Thread.UIThread(o =>
                   {
                       AdExceptionControl errorEx = new AdExceptionControl();
                       errorEx.AdExceptionCode = string.Format("0x{0:X}_{1:X4}", global::System.Math.Abs((long)(error.code / 65536)), global::System.Math.Abs(error.code) % 65536);
                       errorEx.AdExceptionInfo = "异常信息： " + error.message;
                       ShowContext(errorEx, title);
                   });
                }
                else //非网络问题，并且是后台的
                {
                Lin.Core.Utils.Thread.UIThread(o =>
                {
                    ErrorControl errorEx = new ErrorControl();
                    errorEx.ErrorCode = string.Format("0x{0:X}_{1:X4}", global::System.Math.Abs((long)(error.code / 65536)), global::System.Math.Abs(error.code) % 65536);
                    errorEx.ErrorInfo = "异常信息： " + error.message;
                    ShowContext(errorEx, title);
                });
                }
                return;
            }

            #endregion

            
            #region 前端出错
            // 前段本身的异常
            Lin.Util.LinException ad = null;
            try
            {
                ad = type.GetProperty("AdException").GetValue(content, null) as LinException;
            }
            catch
            { };
            if (ad == null)
            {
                ad = content as LinException;
            }
            if (ad != null)
            {
                log.Error(ExceptionInfoToString.ADExceptionToString(ad));
                if (l < _level)
                {
                    return;
                }
                Lin.Core.Utils.Thread.UIThread(o =>
                {
                    AdExceptionControl adControl = new AdExceptionControl();
                    string errorCode = string.Format("0x{0:X}_{1:X4}", global::System.Math.Abs((long)(ad.Code / 65536)), global::System.Math.Abs(ad.Code) % 65536);
                    string info = "异常码： " + errorCode + "\r\n异常信息： " + ad.Message;
                    adControl.AdExceptionCode = errorCode;
                    adControl.AdExceptionInfo = "异常信息： " + ad.Message;
                    ShowContext(adControl, title);
                });

                return;
            }

            #endregion

            #region 系统未知异常

            Exception exception = null;
            try
            {
                exception = type.GetProperty("Exception").GetValue(content, null) as Exception;
            }
            catch
            { }
            if (exception == null)
            {
                exception = content as Exception;
            }
            if (exception != null)
            {
                log.Error(ExceptionInfoToString.ADExceptionToString(exception));
                if (l < _level)
                {
                    return;
                }
                Lin.Core.Utils.Thread.UIThread(o =>
                {
                    ShowContext(new SystemExceptionControl(), title);
                });
                return;
            }
            #endregion

            #region 错误列表(请求出错，将会可能导致的错误原因列出来)

            IList<string> contents = null;
            try
            {
                contents = type.GetProperty("Contents").GetValue(content, null) as IList<string>;
            }
            catch { }
            if (contents == null)
            {
                contents = content as IList<string>;
            }
            if (contents != null && contents.Count > 0)
            {
                Lin.Core.Utils.Thread.UIThread(o =>
                {
                    ErrorContentsControl contentsControl = new ErrorContentsControl();
                    IList<string> list = new List<string>();
                    for (int i = 0; i < contents.Count; i++)
                    {
                        list.Add((i + 1) + ":  " + contents[i] + "\n");
                    }
                    contentsControl.ErrorContents = list;
                    ShowContext(contentsControl, title);
                });

                return;
            }
            #endregion

            #region string字符

            string showContent = null;
            try
            {
                showContent = type.GetProperty("Content").GetValue(content, null) as string;
            }
            catch { }
            if (showContent == null)
            {
                showContent = content as string;
            }
            if (showContent != null)
            {
                Show2(showContent, level, title);
                return;
            }

            #endregion

            Show2(content, level, title);

        }

        /// <summary>
        /// 弹出提示框
        /// </summary>
        /// <param name="contentObj">异常内容</param>
        /// <param name="level">级别</param>
        /// <param name="title">标题</param>
        private static void ShowImpl(object contentObj, LogLevel level = LogLevel.INFO, string title = null)
        {
            int _level = (int)Lin.Core.ViewModel.Context.Global.LogLevel;
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
            TaskbarNotifier objNotifier = new TaskbarNotifier();
            objNotifier.Content = content;
            objNotifier.Title = title;
            objNotifier.Show();
            if (id != 0 && !dict.ContainsKey(id))
            {
                dict.Add(id, objNotifier); // 保存到集合
            }
        }

        /// <summary>
        /// 正常情况下显示提示框窗口
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        private static void ShowObject(string content, string title)
        {
            // 正常提示框窗口
            TaskbarNotifier objNotifier = new TaskbarNotifier();
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
                log.Error(result);
            }
            else if (obj as Error != null)
            {
                result = ExceptionInfoToString.HttpErrorToString(obj as Error);
                httpLog.Error(result);
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
        
        private class ErrorContent
        {
            private string errorCause;
            public string ErrorCause
            {
                set { this.errorCause = value; }
                get { return this.errorCause; }
            }
        }
    }
    /// <summary>
    /// 任务栏提示框
    /// </summary>
    public class TaskbarNotifier : DependencyObject
    {
        internal void ShowSingleWindow(TaskbarNotifier taskbar) // 同样的窗口只显示一个
        {
            TaskbarNotifierWindow taskbarWin = new TaskbarNotifierWindow(taskbar);
        }

        public Window window = null;
        private TaskbarNotifierContent notifierContent = new TaskbarNotifierContent();
        internal bool isFirst = true;
        #region 属性
        /// <summary>
        /// 提示框要显示的标题
        /// </summary>
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(TaskbarNotifier),
            new PropertyMetadata("温馨提示", (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                TaskbarNotifier t = sender as TaskbarNotifier;
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
        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(TaskbarNotifier),
            new PropertyMetadata(null, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                TaskbarNotifier t = sender as TaskbarNotifier;
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
        public readonly static DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(TaskbarNotifierPosition), typeof(TaskbarNotifier),
            new PropertyMetadata(TaskbarNotifierPosition.RightCorner, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            {
                TaskbarNotifier taskbar = (sender as TaskbarNotifier);
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
        public readonly static DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(TaskbarNotifier),
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
        public readonly static DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(TaskbarNotifier),
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
        public readonly static DependencyProperty OpenTimeProperty = DependencyProperty.Register("OpenTime", typeof(int), typeof(TaskbarNotifier),
            new PropertyMetadata(5000, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
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
        public readonly static DependencyProperty HiddingTimeProperty = DependencyProperty.Register("HiddingTime", typeof(int), typeof(TaskbarNotifier),
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
        public readonly static DependencyProperty OpeningTimeProperty = DependencyProperty.Register("OpeningTime", typeof(int), typeof(TaskbarNotifier),
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
        public readonly static DependencyProperty IsAnimationProperty = DependencyProperty.Register("IsAnimation", typeof(bool), typeof(TaskbarNotifier),
            new PropertyMetadata(true, (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
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

        public TaskbarNotifier()
        {
            window = new TaskbarNotifierWindow(this);
        }

        /// <summary>
        /// 弹出提示框窗口
        /// </summary>
        public void Show()
        {
            window.WindowStyle = WindowStyle.None;
            window.Width = Width;
            window.Height = Height;
            if (window.AllowsTransparency == false)
            {
                window.AllowsTransparency = true;
            }
            window.Content = notifierContent;
            window.ShowInTaskbar = false;
            window.Topmost = true;
            (window as TaskbarNotifierWindow).TaskbarNotifierWindow_Loaded();
            window.Show();

        }

        [DefaultValue(LogInfo.LogLevel.INFO)]
        public LogInfo.LogLevel Level { get; set; }

        private void SetLocation()
        {
            switch (Position)
            {
                case TaskbarNotifierPosition.LeftCorner:
                    break;
                case TaskbarNotifierPosition.LeftTop:
                    break;
                case TaskbarNotifierPosition.RightCorner:
                    break;
                case TaskbarNotifierPosition.RightTop:
                    break;
            }
        }

        /// <summary>
        /// 弹出提示框窗口
        /// </summary>
        internal class TaskbarNotifierWindow : Window
        {
            TaskbarNotifier taskbarNotifier = null;
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

            public TaskbarNotifierWindow(TaskbarNotifier task)
            {
                this.stayOpenMilliseconds = task.OpenTime;
                this.openingMilliseconds = task.OpeningTime;
                this.hiddenTop = task.HiddingTime;
                this.isAnimation = task.IsAnimation;
                this.position = task.Position;
                this.MouseLeftButtonDown += TaskbarNotifierWindow_MouseLeftButtonDown;
                this.Background = null;
                taskbarNotifier = task;
            }

            public DisplayStates displayState;
            /// <summary>
            /// 当前的显示状态
            /// </summary>
            public DisplayStates DisplayState
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

            static TaskbarNotifierWindow()
            {
                CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(RoutedCommands.Commands["TaskbarNotifierWindowColose"],
                 (object sender, ExecutedRoutedEventArgs e) =>
                 {
                     TaskbarNotifierWindow notifierWindow = (sender as TaskbarNotifierWindow);
                     notifierWindow.ForceHidden();
                 }));
            }

            /// <summary>
            /// 窗体加载事件(动画效果弹出窗口)
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            public void TaskbarNotifierWindow_Loaded()
            {
                SetInitialLocations(false);
                this.DisplayState = DisplayStates.Hidden;

                // 准备时间器，默认显示多长时间
                this.stayOpenTimer = new DispatcherTimer();
                this.stayOpenTimer.Interval = TimeSpan.FromMilliseconds(this.stayOpenMilliseconds);
                this.stayOpenTimer.Tick += new EventHandler(this.stayOpenTimer_Elapsed);

                // 动画来改变顶部的属性
                this.animation = new DoubleAnimation();
                Storyboard.SetTargetProperty(this.animation, new PropertyPath(Window.TopProperty));
                this.storyboard = new Storyboard();
                this.storyboard.Children.Add(this.animation);
                this.storyboard.FillBehavior = FillBehavior.Stop;
                // 当动画完成时，激活事件，显示或者隐藏
                this.arrivedHidden = new EventHandler(this.Storyboard_ArrivedHidden);
                this.arrivedOpened = new EventHandler(this.Storyboard_ArrivedOpened);
                Notify();
            }

            /// <summary>
            /// 拖拽提示框窗口
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void TaskbarNotifierWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
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
                if (!taskbarNotifier.isFirst && (this.displayState == DisplayStates.Hiding || this.displayState == DisplayStates.Hidden))
                {
                    this.displayState = DisplayStates.Opening;
                }
                if (this.storyboard == null)
                    return;

                this.storyboard.Stop(this); // 停止动画效果

                //由于用于打开和关闭，都可能被重用故事板,已完成的事件处理程序需要被删除
                this.storyboard.Completed -= arrivedHidden;
                this.storyboard.Completed -= arrivedOpened;

                // 处于已经打开状态时
                if (this.displayState == DisplayStates.Opened)
                {
                    taskbarNotifier.isFirst = true;
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
                    taskbarNotifier.isFirst = true;
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
                    taskbarNotifier.isFirst = true;
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
                    //this.ForceHidden();
                }
                else if (this.displayState == DisplayStates.Hidden)
                {
                    taskbarNotifier.isFirst = true;
                    SetInitialLocations(false); // 隐藏窗口
                    this.Visibility = Visibility.Hidden;
                    if (TaskbarNotifierUtil.dict.ContainsKey(TaskbarNotifierUtil.id))
                    {
                        TaskbarNotifierUtil.dict.Remove(TaskbarNotifierUtil.id);
                    }
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
                if (this.Top == destination)
                {
                    return 0;
                }
                double distanceRemaining = global::System.Math.Abs(this.Top - destination);
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
                System.Drawing.Rectangle workingArea = new System.Drawing.Rectangle((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);
                workingArea = WindowForm.Screen.GetWorkingArea(workingArea);
                switch (position)
                {
                    case TaskbarNotifierPosition.LeftCorner:
                        this.Left = 0;
                        this.openedTop = workingArea.Bottom - this.Height;
                        this.hiddenTop = workingArea.Bottom;
                        break;
                    case TaskbarNotifierPosition.LeftTop:
                        this.Left = 0;
                        this.openedTop = 0;
                        this.hiddenTop = workingArea.Top - this.Height;
                        break;
                    case TaskbarNotifierPosition.RightCorner:
                        // 将窗口开始的位置默认为右下角
                        this.Left = workingArea.Right - this.Width;
                        this.openedTop = workingArea.Bottom - this.Height;
                        this.hiddenTop = workingArea.Bottom;
                        break;
                    case TaskbarNotifierPosition.RightTop:
                        this.Left = workingArea.Right - this.Width;
                        this.openedTop = 0;
                        this.hiddenTop = workingArea.Top - this.Height;
                        break;
                }

                // 根据窗口是否打开或者隐藏来设置窗口顶部的位置
                if (showOpened)
                    this.Top = openedTop;
                else
                    this.Top = hiddenTop;
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
                    base.Close();
                };
                st.BeginAnimation(ScaleTransform.ScaleYProperty, cartoonClear);
                st.BeginAnimation(ScaleTransform.ScaleXProperty, cartoonClear);
            }
        }
    }

}
