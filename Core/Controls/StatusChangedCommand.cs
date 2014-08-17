using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "Lin.Core.Controls")]

#endregion
 
namespace Lin.Core.Controls
{
    public static class StatusChangedCommand
    {
        #region  页面加载完毕后的参数
        public static readonly DependencyProperty LoadedCommandParamProperty = DependencyProperty.RegisterAttached("LoadedCommandParam", typeof(object), typeof(StatusChangedCommand),
           new PropertyMetadata(null));

        public static string GetLoadedCommandParam(DependencyObject obj)
        {
            return (string)obj.GetValue(LoadedCommandParamProperty);
        }

        public static void SetLoadedCommandParam(DependencyObject obj, object value)
        {
            obj.SetValue(LoadedCommandParamProperty, value);
        }

        public static readonly DependencyProperty LoadedCommandProperty = DependencyProperty.RegisterAttached("LoadedCommand", typeof(object), typeof(StatusChangedCommand),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                RunTenplate(d);
            }));
        public static object GetLoadedCommand(DependencyObject obj)
        {
            return obj.GetValue(LoadedCommandProperty);
        }
        public static void SetLoadedCommand(DependencyObject obj, object value)
        {
            obj.SetValue(LoadedCommandProperty, value);
        }
        #endregion

        #region 页面退出时候的参数
        public static readonly DependencyProperty UnloadedCommandParamProperty = DependencyProperty.RegisterAttached("UnloadedCommandParam", typeof(object), typeof(StatusChangedCommand),
           new PropertyMetadata(null));

        public static string GetUnloadedCommandParam(DependencyObject obj)
        {
            return (string)obj.GetValue(UnloadedCommandParamProperty);
        }

        public static void SetUnloadedCommandParam(DependencyObject obj, object value)
        {
            obj.SetValue(UnloadedCommandParamProperty, value);
        }

        public static readonly DependencyProperty UnloadedCommandProperty = DependencyProperty.RegisterAttached("UnloadedCommand", typeof(object), typeof(StatusChangedCommand),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                RunTenplate(d);
            }));
        public static object GetUnloadedCommand(DependencyObject obj)
        {
            return obj.GetValue(UnloadedCommandProperty);
        }
        public static void SetUnloadedCommand(DependencyObject obj, object value)
        {
            obj.SetValue(UnloadedCommandProperty, value);
        }
        #endregion

        /// <summary>
        /// 页面显示的时候的参数
        /// </summary>
        public static readonly DependencyProperty VisibleCommandParamProperty = DependencyProperty.RegisterAttached("VisibleCommandParam", typeof(object), typeof(StatusChangedCommand),
           new PropertyMetadata(null));

        public static object GetVisibleCommandParam(DependencyObject obj)
        {
            return obj.GetValue(VisibleCommandParamProperty);
        }

        public static void SetVisibleCommandParam(DependencyObject obj, object value)
        {
            obj.SetValue(VisibleCommandParamProperty, value);
        }

        public static readonly DependencyProperty VisibleCommandProperty = DependencyProperty.RegisterAttached("VisibleCommand", typeof(object), typeof(StatusChangedCommand),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                RunTenplate(d);
            }));
        public static object GetVisibleCommand(DependencyObject obj)
        {
            return obj.GetValue(VisibleCommandProperty);
        }
        public static void SetVisibleCommand(DependencyObject obj, object value)
        {
            obj.SetValue(VisibleCommandProperty, value);
        }

        #region 页面隐藏的时候的参数
        public static readonly DependencyProperty HiddenCommandParamProperty = DependencyProperty.RegisterAttached("HiddenCommandParam", typeof(object), typeof(StatusChangedCommand),
           new PropertyMetadata(null));

        public static string GetHiddenCommandParam(DependencyObject obj)
        {
            return (string)obj.GetValue(HiddenCommandParamProperty);
        }

        public static void SetHiddenCommandParam(DependencyObject obj, object value)
        {
            obj.SetValue(HiddenCommandParamProperty, value);
        }

        public static readonly DependencyProperty HiddenCommandProperty = DependencyProperty.RegisterAttached("HiddenCommand", typeof(object), typeof(StatusChangedCommand),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                RunTenplate(d);
            }));
        public static object GetHiddenCommand(DependencyObject obj)
        {
            return obj.GetValue(HiddenCommandProperty);
        }
        public static void SetHiddenCommand(DependencyObject obj, object value)
        {
            obj.SetValue(HiddenCommandProperty, value);
        }
        #endregion

        public static readonly DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached("DataContext", typeof(object), typeof(StatusChangedCommand),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
            }));
        public static void SetDataContext(DependencyObject obj, object value)
        {
            obj.SetValue(DataContextProperty, value);
        }
        public static object GetDataContext(DependencyObject obj)
        {
            return obj.GetValue(DataContextProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private static void RunTenplate(DependencyObject obj)
        {
            FrameworkElement fe = obj as FrameworkElement;
            fe.Loaded -= FrameworkElementLoaded;
            fe.Loaded += FrameworkElementLoaded;
            fe.Unloaded -= FrameworkElementUnloaded;
            fe.Unloaded += FrameworkElementUnloaded;
            fe.IsVisibleChanged -= IsVisibleChanged;
            fe.IsVisibleChanged += IsVisibleChanged;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">依赖对象</param>
        /// <param name="command">命令参数</param>
        /// <param name="PropertyParam">属性参数</param>
        private static void ExecuteCommand(DependencyObject obj, object command, object PropertyParam)
        {
            if (obj != null)
            {
                FrameworkElement fe = obj as FrameworkElement;
                object dataContext = GetDataContext(fe);
                if (dataContext == null)
                {
                    dataContext = fe.DataContext;
                }
                Lin.Core.ViewModel vm = dataContext as Lin.Core.ViewModel;


                //先判断是否为命令，如果是则执行
                //然后执行VM中的命令
                if (command != null)
                {
                    ICommand tmpCommand = command as ICommand;
                    if (tmpCommand == null && vm != null)
                    {
                        //tmpCommand = vm.Property[command.ToString()] as ICommand;
                        //tmpCommand = vm.Commands[command.ToString()] as ICommand;
                    }
                    if (tmpCommand != null)
                    {
                        if (tmpCommand.CanExecute(PropertyParam))
                        {
                            tmpCommand.Execute(PropertyParam);
                        }
                    }
                }

            }
        }

        private static void FrameworkElementUnloaded(object sender, RoutedEventArgs e)
        {
            ExecuteCommand(sender as DependencyObject, GetUnloadedCommand(sender as DependencyObject), GetUnloadedCommandParam(sender as DependencyObject));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FrameworkElementLoaded(object sender, RoutedEventArgs e)
        {
            ExecuteCommand(sender as DependencyObject, GetLoadedCommand(sender as DependencyObject), GetLoadedCommandParam(sender as DependencyObject));
        }
        private static void IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (true.Equals(e.NewValue))
            {
                ExecuteCommand(sender as DependencyObject, GetVisibleCommand(sender as DependencyObject), GetVisibleCommandParam(sender as DependencyObject));
            }
            else
            {
                ExecuteCommand(sender as DependencyObject, GetHiddenCommand(sender as DependencyObject), GetHiddenCommandParam(sender as DependencyObject));
            }
        }

        private static void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null)
            {
                ExecuteCommand(sender as DependencyObject, GetRenderComand(sender as DependencyObject), GetRenderComandParam(sender as DependencyObject));
                fe.SizeChanged -= SizeChanged;
            }
        }

        /// <summary>
        /// 界面渲染后加载数据
        /// </summary>
        public static readonly DependencyProperty RenderComandParamProperty = DependencyProperty.RegisterAttached("RenderComandParam", typeof(object), typeof(StatusChangedCommand),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
            }));

        public static void SetRenderComandParam(DependencyObject d, object value)
        {
            d.SetValue(RenderComandParamProperty, value);
        }

        public static object GetRenderComandParam(DependencyObject d)
        {
            return d.GetValue(RenderComandParamProperty);
        } 
        public static readonly DependencyProperty RenderComandProperty = DependencyProperty.RegisterAttached("RenderComand", typeof(object), typeof(StatusChangedCommand),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                FrameworkElement fe = d as FrameworkElement;
                if (fe != null)
                {
                    fe.SizeChanged -= SizeChanged;
                    fe.SizeChanged += SizeChanged;
                }
            }));

        public static void SetRenderComand(DependencyObject d, object value)
        {
            d.SetValue(RenderComandProperty, value);
        }

        public static object GetRenderComand(DependencyObject d)
        {
            return d.GetValue(RenderComandProperty);
        }
    }
}
