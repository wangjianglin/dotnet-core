using global::Lin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin
{
    public delegate void CommandLineArgumentsHandle(CommandLineArgumentsArgs args);

    /// <summary>
    /// 程序进入时机
    /// </summary>
    public enum AccessOpportunity
    {
        OnStartup,
        OnStartupNextInstance
    }

    /// <summary>
    /// 命令执行参数类
    /// </summary>
    public class CommandLineArgumentsArgs : System.EventArgs
    {
        public string Name { get; set; }

        public IList<string> ArgsValue { get; set; }

        public AccessOpportunity AccessTime { get; set; }

    }
    /// <summary>
    /// 解析命令行参数
    /// </summary>
    public static class CommandLineArguments
    {
        private static object obj = new object();

        /// <summary>
        /// 程序通过OnStartup进入时的命令行参数
        /// </summary>
        private static IDictionary<string, IList<string>> dictArgs1 = null;//new Dictionary<string, IList<string>>();
        /// <summary>
        /// 程序通过OnStartupNextInstance进入时的命令行参数
        /// </summary>
        private static IDictionary<string, IList<string>> dictArgs2 = null;//new Dictionary<string, IList<string>>();


        /// <summary>
        /// 返回在OnStartup时特定的命令参数
        /// </summary>
        private static IndexProperty<string, IList<string>> _Args = new IndexProperty<string, IList<string>>(name =>
        {
            if (dictArgs1 != null && dictArgs1.ContainsKey(name))
            {
                return dictArgs1[name];
            }
            return null;
        });
        public static IndexProperty<string, IList<string>> Args
        {
            get { return _Args; }
        }


        /// <summary>
        /// 返回在OnStartupNextInstance时特定的命令参数
        /// </summary>
        private static IndexProperty<string, IList<string>> _NextArgs = new IndexProperty<string, IList<string>>(name =>
        {
            if (dictArgs2 != null && dictArgs2.ContainsKey(name))
            {
                return dictArgs2[name];
            }
            return null;
        });
        public static IndexProperty<string, IList<string>> NextArgs
        {
            get { return _NextArgs; }
        }
        /// <summary>
        /// 清除第二次进来时的命令行参数
        /// </summary>
        public static void ClearNextCommandArgs()
        {
            if (dictArgs2 != null)
            {
                dictArgs2.Clear();
            }
        }

        /// <summary>
        /// 返回特定命令的事件
        /// </summary>
        private static Dictionary<string, IList<WeakReference>> dict = new Dictionary<string, IList<WeakReference>>();
        private static IndexProperty<string, CommandLineArgumentsHandle> _Handles = new IndexProperty<string, CommandLineArgumentsHandle>(name =>
        {
            CommandLineArgumentsHandle handle = null;
            foreach (WeakReference item in dict[name])
            {
                if (handle == null)
                {
                    handle = item.Target as CommandLineArgumentsHandle;
                }
                else
                {
                    handle += item.Target as CommandLineArgumentsHandle;
                }
            }
            return handle;
        }, (name, value) =>
        {
            if (dict[name] != null)
            {
                dict[name].Add(new WeakReference(value));
            }
            else
            {
                dict.Add(name, new List<WeakReference>());
                dict[name].Add(new WeakReference(value));
            }
        });

        public static IndexProperty<string, CommandLineArgumentsHandle> Handle
        {
            get { return _Handles; }
        }

        /// <summary>
        /// 事件集合，采用弱引用类型（使用弱引用类型能更好的被垃圾）
        /// </summary>
        private static List<WeakReference> weakList = new List<WeakReference>();

        public static event CommandLineArgumentsHandle Execute
        {
            add
            {
                weakList.Add(new WeakReference(value));
            }
            remove
            {
                lock (obj)
                {
                    for (int i = 0; i < weakList.Count; i++)
                    {
                        if (weakList[i].Target == value)
                        {
                            weakList.Remove(weakList[i]);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="ao"></param>
        public static void Fire(AccessOpportunity ao)
        {
            CommandLineArgumentsHandle handle = null;
            if (ao == AccessOpportunity.OnStartup)
            {
                foreach (KeyValuePair<string, IList<string>> item in dictArgs1)
                {
                    for (int i = 0; i < weakList.Count; i++)
                    {
                        handle = weakList[i].Target as CommandLineArgumentsHandle;
                        if (handle != null)
                        {
                            CommandLineArgumentsArgs args = new CommandLineArgumentsArgs();
                            args.Name = item.Key;
                            args.ArgsValue = item.Value;
                            args.AccessTime = ao;
                            handle(args);
                        }
                        else
                        {
                            weakList.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            else if (ao == AccessOpportunity.OnStartupNextInstance)
            {
                foreach (KeyValuePair<string, IList<string>> item in dictArgs2)
                {
                    for (int i = 0; i < weakList.Count; i++)
                    {
                        handle = weakList[i].Target as CommandLineArgumentsHandle;
                        if (handle != null)
                        {
                            CommandLineArgumentsArgs args = new CommandLineArgumentsArgs();
                            args.Name = item.Key;
                            args.ArgsValue = item.Value;
                            args.AccessTime = ao;
                            handle(args);
                        }
                        else
                        {
                            weakList.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据程序当前运行状态，将处理后的命令参数集合存入对应的集合中去
        /// </summary>
        /// <param name="args"></param>
        public static void ProcessArgs(string[] args, AccessOpportunity ao = AccessOpportunity.OnStartup)
        {
            if (ao == AccessOpportunity.OnStartup)
            {
                if (dictArgs1 == null)
                {
                    Dictionary<string, IList<string>> dict1 = new Dictionary<string, IList<string>>();
                    dictArgs1 = dict1;
                }
                ProcessArgsImpl(args, dictArgs1);
            }
            else if (ao == AccessOpportunity.OnStartupNextInstance)
            {
                if (dictArgs2 == null)
                {
                    Dictionary<string, IList<string>> dict2 = new Dictionary<string, IList<string>>();
                    dictArgs2 = dict2;
                }
                ProcessArgsImpl(args, dictArgs2);
            }
        }

        /// <summary>
        /// 处理命令行参数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static void ProcessArgsImpl(string[] args, IDictionary<string, IList<string>> dictArgs)
        {
            IList<string> logs = null;
            string[] escapeArgs= EscapeCode(args);
            if (escapeArgs.Length > 0)
            {
                string argName = "";
                for (int i = 0; i < escapeArgs.Length; i++)
                {
                    if (escapeArgs[i][0] == '-' && escapeArgs[i].Length > 1)
                    {
                        argName = escapeArgs[i].Substring(1, escapeArgs[i].Length - 1);
                        if (!dictArgs.ContainsKey(argName))
                        {
                            logs = new List<string>();
                        }
                        else
                        {
                            logs = dictArgs[argName];
                        }
                        try
                        {
                            if ((i < escapeArgs.Length - 1) && !escapeArgs[i + 1].StartsWith("-"))
                            {
                                string value = escapeArgs[i + 1];
                                value = value.Replace("\\-", "-");
                                logs.Add(value);
                                i++;
                            }
                            else
                            {
                                logs.Add("");
                            }
                        }
                        catch
                        {
                            dictArgs.Clear();
                            //throw new AdException(0X2002012, "命令行参数处理出错!");
                            throw new Exception("命令行参数处理出错!");
                        }
                        if (!dictArgs.ContainsKey(argName))
                        {
                            dictArgs.Add(argName, logs);
                        }
                    }
                    else
                    {
                        dictArgs.Clear();
                        //throw new AdException(0X2002011, "输入的命令行参数格式不正确,该命令行将失效！");
                        throw new Exception("输入的命令行参数格式不正确,该命令行将失效！");
                    }
                }
            }
        }

        /// <summary>
        /// 处理转义
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string[] EscapeCode(string[] args)
        {
            string[] escapeArgs = new string[args.Length];
            StringBuilder arg = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                for (int j = 0; j < args[i].Length; j++)
                {
                    if (args[i][j] == '\\')
                    {
                        if (args[i][j + 1] == 'n')
                        {
                            arg.Append((char)10);
                            j++;
                        }
                        else if (args[i][j + 1] == 'r')
                        {
                            arg.Append((char)13);
                            j++;
                        }
                        else if (args[i][j + 1] == 't')
                        {
                            arg.Append((char)9);
                            j++;
                        }
                        else if (args[i][j + 1] == '\\')
                        {
                            arg.Append((char)92);
                            j++;
                        }
                        else if (args[i][j + 1] == '\'')
                        {
                            arg.Append((char)39);
                            j++;
                        }
                        else if (args[i][j + 1] == '"')
                        {
                            arg.Append((char)34);
                            j++;
                        }
                        else if (args[i][j + 1] == '0')
                        {
                            arg.Append((char)0);
                            j++;
                        }
                        else if (args[i][j + 1] == '-')
                        {
                            arg.Append((char)45);
                            j++;
                        }
                    }
                    else
                    {
                        arg.Append(args[i][j]);
                    }
                }
                escapeArgs[i] = arg.ToString();
                arg = new StringBuilder();
            }
            return escapeArgs;
        }
    }
}
