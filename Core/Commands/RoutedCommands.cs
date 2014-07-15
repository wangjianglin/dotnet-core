using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;

namespace Lin.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class RoutedCommands
    {
        /// <summary>
        /// 执行指定的路由命令)
        /// </summary>
        /// <param name="CommandName"></param>
        /// <param name="Param"></param>
        /// <param name="Element"></param>
        public static void Execute(string CommandName, object Param, IInputElement Element)
        {
            Lin.Core.Utils.Thread.UIThread(obj =>
            {
                RoutedCommands.Commands[CommandName].Execute(Param, Element);
            }, null);
        }

        /// <summary>
        /// 命令可以注册多次，根据类型type，执行不同对象注册的命令
        /// </summary>
        private static IDictionary<Type, IList<RegisterCommandBindings>> binds = new Dictionary<Type, IList<RegisterCommandBindings>>();
        public static void RegisterClassCommandBinding(Type type, RegisterCommandBindings commandBinding)
        {
            IList<RegisterCommandBindings> commandList = null;
            if (!binds.ContainsKey(type))//type类型的第一个路由命令注册
            {
                commandList = new List<RegisterCommandBindings>();
                binds.Add(type, commandList);

                //注册路由命令
                CommandBinding c = new CommandBinding(commandBinding.Command, ExecutedRoutedEventHandler(type), CanExecuteRoutedEventHandler(type));
                // c.PreviewCanExecute += c_PreviewCanExecute(type);
                //c.PreviewExecuted += c_PreviewExecuted(type);

                CommandManager.RegisterClassCommandBinding(type, c);
            }
            else
            {
                commandList = binds[type];
            }

            //如果type类型已经被注册过，则添加路由命令到type类型的命令集合中
            if (!commandList.Contains(commandBinding))
            {
                commandList.Add(commandBinding);
            }
        }

        /// <summary>
        /// 移除type类型中，指定的路由命令
        /// </summary>
        /// <param name="type">路由命令的类型</param>
        /// <param name="commandBinding">命令</param>
        public static void RemoveRegisterClassCommandBinding(Type type, RegisterCommandBindings commandBinding)
        {
            IList<RegisterCommandBindings> commandList = null;
            if (binds.ContainsKey(type))
            {
                commandList = binds[type];
                if (commandList.Count > 0)
                {
                    commandList.Remove(commandBinding);
                }
            }
        }

        /// <summary>
        /// 当调用Execute方法时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static ExecutedRoutedEventHandler c_PreviewExecuted(Type type)
        {
            return new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
            {
                if (binds.ContainsKey(type))
                {
                    foreach (RegisterCommandBindings cn in binds[type])
                    {
                        cn.FirePreviewExecuted(sender, e);
                    }
                }
            });
        }

        /// <summary>
        /// 当调用CanExecute方法时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static CanExecuteRoutedEventHandler c_PreviewCanExecute(Type type)
        {
            return new CanExecuteRoutedEventHandler((object sender, CanExecuteRoutedEventArgs e) =>
            {
                if (binds.ContainsKey(type))
                {
                    foreach (RegisterCommandBindings cn in binds[type])
                    {
                        cn.FirePreviewCanExecute(sender, e);
                    }
                }
            });
        }

        /// <summary>
        /// 是否可以执行路由事件(当调用 RoutedCommand 上的 CanExecute 方法并且未处理 PreviewCanExecute 事件时发生)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static CanExecuteRoutedEventHandler CanExecuteRoutedEventHandler(Type type)
        {
            return new CanExecuteRoutedEventHandler((object sender, CanExecuteRoutedEventArgs e) =>
            {
                e.CanExecute = true;
                e.ContinueRouting = true;
                if (binds.ContainsKey(type))
                {
                    foreach (RegisterCommandBindings cn in binds[type])
                    {
                        cn.FireCanExecute(sender, e);
                    }
                }
            });
        }

        /// <summary>
        /// 执行指定对象的路由事件(当调用 RoutedCommand 上的 Execute 方法并且未处理 PreviewExecuted 事件时发生。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static ExecutedRoutedEventHandler ExecutedRoutedEventHandler(Type type)//
        {
            return new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs args) =>
            {
                //Type type = sender.GetType();
                if (binds.ContainsKey(type))
                {
                    foreach (RegisterCommandBindings cn in binds[type])
                    {
                        cn.FireExecute(sender, args);
                    }
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RoutedCommandProperty = DependencyProperty.RegisterAttached("RoutedCommand", typeof(RoutedCommand), typeof(RoutedCommands),
           new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
           {
               CommandManager.RegisterClassCommandBinding(d.GetType(), new System.Windows.Input.CommandBinding(e.NewValue as RoutedCommand,
                   (object sender, ExecutedRoutedEventArgs er) =>
                   {
                       if (e.NewValue != null)
                       {
                           ICommand command = GetCommand(sender as DependencyObject);
                           if (command != null)
                           {
                               command.Execute(er.Parameter);
                           }
                       }
                   }));
           }));
        public static void SetRoutedCommand(DependencyObject obj, RoutedCommand value)
        {
            obj.SetValue(RoutedCommandProperty, value);
        }
        public static RoutedCommand GetRoutedCommand(DependencyObject obj)
        {
            return (RoutedCommand)obj.GetValue(RoutedCommandProperty);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(RoutedCommands),
           new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
           {

           }));
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }


        //public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(RoutedCommands),
        //   new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        //   {
        //   }));
        //public static void SetCommandParameter(DependencyObject obj, object value)
        //{
        //    obj.SetValue(CommandParameterProperty, value);
        //}
        //public static object GetCommandParameter(DependencyObject obj)
        //{
        //    return obj.GetValue(CommandParameterProperty);
        //}


        private static RoutedCommandsProperty commands = new RoutedCommandsProperty();

        public static RoutedCommandsProperty Commands
        {
            get { return commands; }
        }

        private static RoutedCommand _Tmp = new System.Windows.Input.RoutedCommand();
        public static RoutedCommand Tmp
        {
            get
            {
                return _Tmp;
            }
        }


        public static System.Windows.Input.RoutedCommand InitRoutedCommand(string command)
        {
            System.Windows.Input.RoutedCommand c = new System.Windows.Input.RoutedCommand();
            commands.IintValue(command, c);
            return c;
        }

        public class RoutedCommandsProperty
        {
            public event PropertyChangedEventHandler PropertyChanged;
            internal RoutedCommandsProperty()
            {
            }
            private Dictionary<string, System.Windows.Input.RoutedCommand> _Commands = new Dictionary<string, System.Windows.Input.RoutedCommand>();
            public System.Windows.Input.RoutedCommand this[string s]
            {
                get
                {
                    if (s == null || s == "")
                    {
                        return null;
                    }
                    if (_Commands.ContainsKey(s))
                    {
                        return _Commands[s];
                    }
                    return InitRoutedCommand(s);
                }
                set
                {
                    this.AddValue(s, value);
                }
            }
            private void AddValue(string commandName, System.Windows.Input.RoutedCommand command)
            {
                if (commandName == null || commandName == "")
                {
                    return;
                }
                object pre = null;
                if (_Commands.ContainsKey(commandName))
                {
                    pre = _Commands[commandName];
                    if (command == null)
                    {
                        _Commands.Remove(commandName);
                    }
                    else
                    {
                        _Commands[commandName] = command;
                    }
                }
                else if (command != null)
                {
                    _Commands.Add(commandName, command);
                }
                if ((pre == null && command != null) || (pre != null && !pre.Equals(command)))
                {
                    PropertyChangedEventHandler handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        var e = new PropertyChangedEventArgs(commandName);
                        handler(this, e);
                    }
                }
            }
            internal void IintValue(string commandName, System.Windows.Input.RoutedCommand command)
            {
                if (commandName == null || commandName == "")
                {
                    return;
                }
                if (_Commands.ContainsKey(commandName))
                {
                    if (command == null)
                    {
                        _Commands.Remove(commandName);
                    }
                    else
                    {
                        _Commands[commandName] = command;
                    }
                }
                else if (command != null)
                {
                    _Commands.Add(commandName, command);
                }
            }
        }
    }
}
