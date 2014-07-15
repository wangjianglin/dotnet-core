using System;
using System.ComponentModel;
using System.Diagnostics;
using Lin.Core.Web.Json;
using System.Dynamic;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;
using Lin.Util;

namespace Lin.Core.ViewModel
{
    /// <summary>
    /// 
    /// 1、实现把命令绑定到方法
    /// 2、属性动态扩展
    /// 3、实现INotifyPropertyChanged
    /// 
    /// </summary>
    [Serializable]
    [JsonSkip]
    public abstract class ViewModelBase : System.Dynamic.DynamicObject, INotifyPropertyChanged, IDisposable, ICustomTypeDescriptor
    {
        
        #region Constructor

        public ViewModelBase()
        {
            InitPropetyChangedInfos();
        }

        #endregion // Constructor

        #region DisplayName

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
         [JsonSkip]
        public virtual string DisplayName { get; protected set; }

        #endregion // DisplayName

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            //if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            //{
            //    string msg = "Invalid property name: " + propertyName;

            //    if (this.ThrowOnInvalidPropertyName)
            //        throw new Exception(msg);
            //    else
            //        Debug.Fail(msg);
            //}
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members


        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
         [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        protected void FirePropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members 
        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        #endregion // IDisposable Members

        #region ICustomTypeDescriptor

        protected void CanExecuteChanged(string name)
        {

        }

        private IDictionary<string,MethodInfo> getCommandCanExceuteInfos()
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

        private PropertyDescriptorCollection getCommandInfos()
        {
            IDictionary<string, MethodInfo> canExceuteInfo = getCommandCanExceuteInfos();
            Type type = this.GetType();
            //MethodInfo[] ms = type.GetMethods();
            MethodInfo[] ms = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            ParameterInfo param = null;
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
                        list.Add(new ViewModelCommandPropertyDescriptor(this, command.Name, m, canExceuteInfo[command.Name]));
                    }
                    else
                    {
                        list.Add(new ViewModelCommandPropertyDescriptor(this, command.Name, m, null));
                    }
                }
            }
            return new PropertyDescriptorCollection(list.ToArray());
        }

       

        public AttributeCollection GetAttributes()
        {
            throw new NotImplementedException();
        }

        public string GetClassName()
        {
            throw new NotImplementedException();
        }

        public string GetComponentName()
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            //throw new NotImplementedException();
            //PropertyDescriptorCollection collection = new PropertyDescriptorCollection();
            PropertyDescriptorCollection commands = getCommandInfos();
            return commands;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region DynamicObject

        protected dynamic self
        {
            get { return this; }
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
                PropertyChanaged[] attributes = m.GetCustomAttributes(typeof(PropertyChanaged), true) as PropertyChanaged[];
                if (attributes != null && attributes.Length > 0)
                {
                    ps = m.GetParameters();
                    if (ps != null && ps.Length > 2)
                    {
                        LinException.FireWarning(this, new LinException(0x800.ExceptionCode(0x0001), type.Name + ":" + m.Name + " Too many parameters."));
                        continue;
                    }
                    if(ps == null){
                        paramCount = 0;
                    }
                    paramCount = ps.Length;
                }
                foreach (PropertyChanaged propertyChanaged in attributes)
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
        /// <summary>
        /// 存储属性值
        /// </summary>
        private IDictionary<string, Object> properties = new Dictionary<string, Object>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            //return base.TrySetMember(binder, value);
            Object oldValue = null;
            if (properties.ContainsKey(binder.Name))
            {
                oldValue = properties[binder.Name];
                properties[binder.Name] = value;
            }
            else
            {
                properties.Add(binder.Name, value);
            }
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
                        m.Invoke(this, new Object[] { value,oldValue });
                        break;
                }
            }
            this.OnPropertyChanged(binder.Name);
            return true;
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            //return base.TryGetMember(binder, out result);
            result = null;
            if (properties.ContainsKey(binder.Name))
            {
                result = properties[binder.Name];
                return true;
            }
            return false;
        }
        #endregion
    }
}