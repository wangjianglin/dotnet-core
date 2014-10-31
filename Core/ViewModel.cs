using System;
using System.ComponentModel;
using System.Diagnostics;
using Lin.Util.Json;
using System.Dynamic;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;
using Lin.Util;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Lin.Util.Json;

namespace Lin.Core
{
    //public class ViewModelCommandPropertyDescriptor : PropertyDescriptor
    //{
    //    private DynamicModel vm;
    //    private string name;
    //    private MethodInfo method;
    //    private ICommand commmand = null;
    //    public ViewModelCommandPropertyDescriptor(ViewModel vm, string name, MethodInfo method, MethodInfo canExecuteMethod)
    //        : base(name, null)
    //    {
    //        Predicate<object> canExecute = null;
    //        if (canExecuteMethod != null)
    //        {
    //            canExecute = obj =>
    //            {
    //                bool r = true;
    //                if (canExecuteMethod.GetParameters().Length == 0)
    //                {
    //                    r = (bool)canExecuteMethod.Invoke(vm, null);
    //                }
    //                else
    //                {
    //                    r = (bool)canExecuteMethod.Invoke(vm, new Object[] { obj });
    //                }
    //                return r;
    //            };
    //        }
    //        this.vm = vm;
    //        this.name = name;
    //        this.method = method;
    //        commmand = new Lin.Core.Commands.ReplayCommand(obj =>
    //        {
    //            if (method.GetParameters().Length == 0)
    //            {
    //                method.Invoke(vm, null);
    //            }
    //            else
    //            {
    //                method.Invoke(vm, new object[] { obj });
    //            }
    //        }, canExecute);
    //    }
    //    public override bool CanResetValue(object component)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override Type ComponentType
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override object GetValue(object component)
    //    {
    //        return commmand;
    //    }

    //    public override bool IsReadOnly
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override Type PropertyType
    //    {
    //        get { return typeof(ICommand); }
    //    }

    //    public override void ResetValue(object component)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void SetValue(object component, object value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool ShouldSerializeValue(object component)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    //public class ViewModelPropertyDescriptor2 : PropertyDescriptor
    //{
    //    private object value;
    //    public ViewModelPropertyDescriptor2(string name,object value)
    //        : base(name, null)
    //    {
    //        this.value = value;
    //    }
    //    public override bool CanResetValue(object component)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override Type ComponentType
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override object GetValue(object component)
    //    {
    //        return this.value;
    //    }

    //    public override bool IsReadOnly
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override Type PropertyType
    //    {
    //        get { if (value != null) { return value.GetType(); } else { return typeof(object); } }
    //    }

    //    public override void ResetValue(object component)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void SetValue(object component, object value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool ShouldSerializeValue(object component)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    /// <summary>
    /// 
    /// 1、实现把命令绑定到方法
    /// 2、属性动态扩展
    /// 3、实现INotifyPropertyChanged
    /// 4、属性改变绑定到方法
    /// 
    /// </summary>
    [Serializable]
    [JsonSkip]
    public class ViewModel : DynamicModel
    {
        
        public ViewModel()
        {
            //InitPropetyChangedInfos();
            InitPropetyChangedInfos();
            InitCommands();
        }
        
        private IDictionary<string, MethodInfo> propertyChangedInfos = new Dictionary<string, MethodInfo>();
        private IDictionary<string, int> propertyChangedParamsInfos = new Dictionary<string, int>();

        private void InitPropetyChangedInfos()
        {
            Type type = this.GetType();
            //MethodInfo[] ms = type.GetMethods();
            MethodInfo[] ms = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            ParameterInfo[] ps = null;
            int paramCount;
            foreach (MethodInfo m in ms)
            {
                paramCount = 0;
                PropertyChanged[] attributes = m.GetCustomAttributes(typeof(PropertyChanged), true) as PropertyChanged[];
                if (attributes != null && attributes.Length > 0)
                {
                    ps = m.GetParameters();
                    if (ps != null && ps.Length > 2)
                    {
                        LinException.FireWarning(this, new LinException(0x800.ExceptionCode(0x0001), type.Name + ":" + m.Name + " Too many parameters."));
                        continue;
                    }
                    if (ps == null)
                    {
                        paramCount = 0;
                    }
                    paramCount = ps.Length;
                }
                foreach (PropertyChanged propertyChanaged in attributes)
                {
                    if (propertyChangedInfos.ContainsKey(propertyChanaged.Name))
                    {
                        LinException.FireWarning(this, new LinException(0x800.ExceptionCode(0x0002), type.Name + ":" + m.Name + "[" + propertyChanaged.Name + "] repeat."));
                        continue;
                    }
                    propertyChangedInfos.Add(propertyChanaged.Name, m);
                    propertyChangedParamsInfos.Add(propertyChanaged.Name, paramCount);
                }
            }
        }

        ////private List<string> names = new List<string>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            //return base.TrySetMember(binder, value);

            //base.TryGetMember(new GetMemberBinder(binder.Name,binder.IgnoreCase), ref oldValue);
            base.TrySetMember(binder, value);
            if (propertyChangedInfos.ContainsKey(binder.Name))
            {
                MethodInfo m = propertyChangedInfos[binder.Name];
                //if(m.GetParameters())
                int paramCount = propertyChangedParamsInfos[binder.Name];
                switch (paramCount)
                {
                    case 0:
                        m.Invoke(this, null);
                        break;
                    case 1:
                        m.Invoke(this, new Object[] { value });
                        break;
                    case 2:
                        m.Invoke(this, new Object[] { value, values[binder.Name] });
                        break;
                }
            }
            return true;
        }

        #region command

        private void InitCommands()
        {
            IDictionary<string, MethodInfo> canExceuteInfo = GetCommandCanExceuteInfos();
            Type type = this.GetType();
            //MethodInfo[] ms = type.GetMethods();
            MethodInfo[] ms = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            //ParameterInfo param = null;
            ParameterInfo[] ps = null;
            IDictionary<string, MethodInfo> infos = new Dictionary<string, MethodInfo>();

            List<PropertyDescriptor> list = new List<PropertyDescriptor>();
            foreach (MethodInfo m in ms)
            {
                Command[] attributes = m.GetCustomAttributes(typeof(Command), true) as Command[];
                if (attributes != null && attributes.Length > 0)
                {
                    ps = m.GetParameters();
                    if (ps != null && ps.Length > 1)
                    {
                        LinException.FireWarning(this, new LinException(0x800.ExceptionCode(0x0001), type.Name + ":" + m.Name + " Too many parameters."));
                        continue;
                    }
                }
                foreach (Command command in attributes)
                {
                    if (infos.ContainsKey(command.Name))
                    {
                        LinException.FireWarning(this, new LinException(0x800.ExceptionCode(0x0001), type.Name + ":" + m.Name + "[" + command.Name + "] repeat."));
                        continue;
                    }
                    infos.Add(command.Name, m);
                    if (canExceuteInfo.ContainsKey(command.Name))
                    {
                        //list.Add(new ViewModelCommandPropertyDescriptor(this, command.Name, m, canExceuteInfo[command.Name]));
                        this.GenerCommand(this, command.Name, m, canExceuteInfo[command.Name]);
                    }
                    else
                    {
                        //list.Add(new ViewModelCommandPropertyDescriptor(this, command.Name, m, null));
                        this.GenerCommand(this, command.Name, m, null);
                    }
                }
            }

            //    return new PropertyDescriptorCollection(list.ToArray());
        }

        protected void FireCommandCanExecuteChanged(string name)//,[CallerMemberName] string memberName = "")
        {
            //StackFrame frame = new StackTrace().GetFrames()[0];
            //MethodAttributes frame.GetMethod().Attributes;
            Lin.Core.Commands.ReplayCommand command = values[name] as Lin.Core.Commands.ReplayCommand;
            if (command != null)
            {
                command.FireCanExecute();
            }
        }
        private void GenerCommand(ViewModel vm, string name, MethodInfo method, MethodInfo canExecuteMethod)
        {
            Predicate<object> canExecute = null;
            if (canExecuteMethod != null)
            {
                canExecute = obj =>
                {
                    bool r = true;
                    if (canExecuteMethod.GetParameters().Length == 0)
                    {
                        r = (bool)canExecuteMethod.Invoke(vm, null);
                    }
                    else
                    {
                        r = (bool)canExecuteMethod.Invoke(vm, new Object[] { obj });
                    }
                    return r;
                };
            }
            //this.vm = vm;
            //this.name = name;
            //this.method = method;
            values[name] = new Lin.Core.Commands.ReplayCommand(obj =>
            {
                if (method.GetParameters().Length == 0)
                {
                    method.Invoke(vm, null);
                }
                else
                {
                    method.Invoke(vm, new object[] { obj });
                }
            }, canExecute);
        }
        
        private IDictionary<string, MethodInfo> GetCommandCanExceuteInfos()
        {
            Type type = this.GetType();
            //MethodInfo[] ms = type.GetMethods();
            MethodInfo[] ms = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            ParameterInfo param = null;
            ParameterInfo[] ps = null;
            IDictionary<string, MethodInfo> infos = new Dictionary<string, MethodInfo>();

            List<PropertyDescriptor> list = new List<PropertyDescriptor>();
            Type rType;
            foreach (MethodInfo m in ms)
            {
                CommandCanExecute[] attributes = m.GetCustomAttributes(typeof(CommandCanExecute), true) as CommandCanExecute[];
                if (attributes != null && attributes.Length > 0)
                {
                    ps = m.GetParameters();
                    if (ps == null || ps.Length == 0)
                    {
                        param = null;
                    }
                    else if (ps.Length == 1)
                    {
                        param = ps[0];
                    }
                    else
                    {
                        //LinException.FireWarning(this, new LinException(0x800.ExceptionCode(0x0001), type.Name + ":" + m.Name + " Too many parameters."));
                        //改为抛异常的方式
                        continue;
                    }
                    rType = m.ReflectedType;
                    if (rType != typeof(bool) && rType != typeof(Boolean))
                    {
                        //抛异常
                    }
                }
                foreach (CommandCanExecute commandCanExceute in attributes)
                {
                    if (infos.ContainsKey(commandCanExceute.Name))
                    {
                        LinException.FireWarning(this, new LinException(0x800.ExceptionCode(0x0001), type.Name + ":" + m.Name + "[" + commandCanExceute.Name + "] repeat."));
                        continue;
                    }
                    infos.Add(commandCanExceute.Name, m);
                    //list.Add(new ViewModelCommandPropertyDescriptor(this, command.Name, m, param, null));
                }
            }
            return infos;
        }

        //private PropertyDescriptorCollection getCommandInfos()
        //{
        //    IDictionary<string, MethodInfo> canExceuteInfo = getCommandCanExceuteInfos();
        //    Type type = this.GetType();
        //    //MethodInfo[] ms = type.GetMethods();
        //    MethodInfo[] ms = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        //    ParameterInfo param = null;
        //    ParameterInfo[] ps = null;
        //    IDictionary<string, MethodInfo> infos = new Dictionary<string, MethodInfo>();

        //    List<PropertyDescriptor> list = new List<PropertyDescriptor>();
        //    foreach (MethodInfo m in ms)
        //    {
        //        Command[] attributes = m.GetCustomAttributes(typeof(Command), true) as Command[];
        //        if (attributes != null && attributes.Length > 0)
        //        {
        //            ps = m.GetParameters();
        //            if (ps != null && ps.Length > 1)
        //            {
        //                LinException.FireWarning(this, new LinException(0x800.ExceptionCode(0x0001), type.Name + ":" + m.Name + " Too many parameters."));
        //                continue;
        //            }
        //        }
        //        foreach (Command command in attributes)
        //        {
        //            if (infos.ContainsKey(command.Name))
        //            {
        //                LinException.FireWarning(this, new LinException(0x800.ExceptionCode(0x0001), type.Name + ":" + m.Name + "[" + command.Name + "] repeat."));
        //                continue;
        //            }
        //            infos.Add(command.Name, m);
        //            if (canExceuteInfo.ContainsKey(command.Name))
        //            {
        //                list.Add(new ViewModelCommandPropertyDescriptor(this, command.Name, m, canExceuteInfo[command.Name]));
        //            }
        //            else
        //            {
        //                list.Add(new ViewModelCommandPropertyDescriptor(this, command.Name, m, null));
        //            }
        //        }
        //    }

        //    return new PropertyDescriptorCollection(list.ToArray());
        //}
         
       

        //override public PropertyDescriptorCollection GetProperties()
        //{
        //    //throw new NotImplementedException();
        //    //PropertyDescriptorCollection collection = new PropertyDescriptorCollection();
        //    PropertyDescriptorCollection commands = getCommandInfos();
        //    return commands;
        //}


        #endregion


   }
}

#region old

/// <summary>
/// 实现数据验证
/// </summary>
//[Serializable]
//[JsonSkip]
//public class ViewModel : DynamicModel//,IDataErrorInfo,IDataValidationInfo
//{
    //[JsonSkip]
    //public Dictionary<string, string> errors = new Dictionary<string, string>();
    //#region IDataErrorInfo Members

    //public string Error
    //{
    //    get { return null; }
    //}

    //public string this[string columnName]
    //{
    //    get
    //    {
    //        //Lin.Core.Controls.TaskbarNotifierUtil.Show(columnName);
    //        if (errors.ContainsKey(columnName))
    //        {
    //            //Lin.Core.Controls.TaskbarNotifierUtil.Show(columnName);
    //            return errors[columnName];
    //        }
    //        else
    //        {
    //            return string.Empty; 
    //        } 
    //    }
    //}

    //#endregion

    ////设置错误信息
    //public virtual void SetError(string propertyName, string errorMessage)
    //{
    //    errors[propertyName] = errorMessage;
    //    NotifyPropertyChanged(propertyName);
    //}
    ////清除错误信息
    //public virtual void ClearError(string propertyName)
    //{
    //    errors.Remove(propertyName);
    //    this.OnPropertyChanged(propertyName);
    //}

    //public ViewModel()
    //{
    //}


    //#region 实现数据验证

    /// <summary>
    /// 
    /// </summary>
    //private Dictionary<string, object> DataValidationErrors = new Dictionary<string, object>();
    ///// <summary>
    ///// 添加错误信息
    ///// </summary>
    ///// <param name="propertyName"></param>
    ///// <param name="content"></param>
    //public void Added(string propertyName, object content)
    //{
    //    if (!DataValidationErrors.ContainsKey(propertyName))
    //    {
    //        DataValidationErrors.Add(propertyName, content);
    //    }
    //    else
    //    {
    //        DataValidationErrors[propertyName] = content;
    //    }
    //    if (ValidationChanged != null)
    //    {
    //        ValidationChanged(this, new EventArgs());
    //    }
    //}
    ///// <summary>
    ///// 删除错误信息
    ///// </summary>
    ///// <param name="propertyName"></param>
    //public void Removed(string propertyName)
    //{
    //    if (DataValidationErrors.ContainsKey(propertyName))
    //    {
    //        DataValidationErrors.Remove(propertyName);
    //    }
    //    if (ValidationChanged != null)
    //    {
    //        ValidationChanged(this, new EventArgs());
    //    }
    //}
    ///// <summary>
    ///// 返回当前是验证是否通过,true为有错误信息未通过，false为无错误信息验证通过
    ///// </summary>
    ///// <returns></returns>
    //public bool HasError()
    //{
    //    if (DataValidationErrors.Count > 0)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    //public event EventHandler ValidationChanged;

    //#endregion
//}

#endregion