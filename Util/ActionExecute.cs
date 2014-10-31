using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lin.Util
{
    public enum ActionExecuteType
    {
        FIRST, END
    }

    /// <summary>
    /// 
    /// </summary>
    public class ActinExceuteExcepton : Exception
    {
        public IList<Exception> Exceptions { get; private set; }
        public ActinExceuteExcepton(List<Exception> Exceptions)
        {
            this.Exceptions = Exceptions;
        }
    }

    /// <summary>
    /// Action执行器，确保每个Action都会被执行，不管Action执行是否有异常，如果有异常，会在所有Action执行完之后，再进入异常处理程序
    /// </summary>
    public class ActionExecute : IDisposable
    {
        private ActionExecuteType type;
        private volatile Action action;
        private volatile Action executingAction;
        private object actionLock = new object();
        private object executingActionLock = new object();
        private System.Threading.Thread thread;

        private static void ExecuteImpl(System.Action[] actions, int n,List<Exception> list)
        {
            try
            {
                actions[n]();
            }
            catch (Exception e)
            {
                list.Add(e);
            }
            finally
            {
                n++;
                if (n < actions.Length)
                {
                    ExecuteImpl(actions, n,list);
                }
            }
        }

        /// <summary>
        /// 依次执行actions中的Action，确保每个Action都会被执行
        /// </summary>
        /// <param name="actions"></param>
        public static void Execute(params System.Action[] actions)
        {
            if (actions != null && actions.Length > 0)
            {
                List<Exception> list = new List<Exception>();
                ExecuteImpl(actions, 0, list);
                if (list.Count() > 0)
                {
                    throw new ActinExceuteExcepton(list);
                }
            }
        }
        private volatile bool isRun = true;

        /// <summary>
        /// 根据Action执行的类型去判断，最终只执行哪个请求
        /// </summary>
        /// <param name="actionType">执行操作的类型</param>
        public ActionExecute(ActionExecuteType actionType)
        {
            type = actionType;
            thread = new System.Threading.Thread(obj =>
           {
               while (isRun)
               {
                   ExecutingAction();
                   System.Threading.Thread.Sleep(1);
               }
           });
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 根据Action的类型来执行不同的操作
        /// </summary>
        private void ExecutingAction()
        {
            if (type == ActionExecuteType.END)
            {
                lock (actionLock)
                {
                    executingAction = action;
                    action = null;
                }
                if (executingAction != null)
                {
                    executingAction();
                }
            }
            else
            {
                if (executingAction != null)
                {
                    executingAction();
                    lock (executingActionLock)
                    {
                        executingAction = null;
                    }
                }
            }
        }

        /// <summary>
        /// 将Action保存到集合中
        /// </summary>
        /// <param name="action"></param>
        public void Post(Action action)
        {
            if (type == ActionExecuteType.END)
            {
                lock (action)
                {
                    this.action = action;
                }
            }
            else
            {
                lock (executingActionLock)
                {
                    if (executingAction == null)
                    {
                        executingAction = action;
                    }
                }
            }
        }

        /// <summary>
        /// 终止线程
        /// </summary>
        public void Dispose()
        {
            isRun = false;
        }
    }
}
