using Lin.Core.Web.Json;
using Lin.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lin.Core
{

    /// <summary>
    /// 
    /// </summary>
    [JsonSkip]
    public class DynamicModel : System.Dynamic.DynamicObject, INotifyPropertyChanged, IDisposable//,ICustomTypeDescriptor
    {
        #region Constructor

        public DynamicModel()
        {
            //InitPropetyChangedInfos();
            InitDynamicModel();
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
        ~DynamicModel()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        #endregion // IDisposable Members



        #region DynamicObject

        protected dynamic self
        {
            get { return this; }
        }

        private void InitDynamicModel()
        {
            this.values = new IndexProperty<string, object>(name =>
            {
                return this.GetValue(name);
            }, (name, value) =>
            {
                this.SetValue(name, value);
            });
        }
        protected IndexProperty<string, object> values { get; private set; }

        /// <summary>
        /// 存储属性值
        /// </summary>
        protected IDictionary<string, Object> properties = new Dictionary<string, Object>();

        private void SetValue(string name, object value)
        {
            if (properties.ContainsKey(name))
            {
                //oldValue = properties[binder.Name];
                properties[name] = value;
            }
            else
            {
                properties.Add(name, value);
            }

            this.OnPropertyChanged(name);
        }
        private object GetValue(string name)
        {
            if (properties.ContainsKey(name))
            {
                return properties[name];
            }
            return null;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            //return base.TrySetMember(binder, value);
            //Object oldValue = null;
            this.SetValue(binder.Name, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            //return base.TryGetMember(binder, out result);
            result = GetValue(binder.Name);
            return result == null ? false : true;
        }

        //public override DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        //{
        //    return base.GetMetaObject(parameter);

        //    //return new DynamicModelMetaObject(parameter,this);
        //    //return new DynamicMetaObject(
        //    //    parameter,
        //    //    BindingRestrictions.GetTypeRestriction(
        //    //        parameter,
        //    //        typeof(DynamicModel)));
        //}

        //public override IEnumerable<string> GetDynamicMemberNames()
        //{
        //    return base.GetDynamicMemberNames();
        //}

        #endregion



        #region ICustomTypeDescriptor

        //protected void CanExecuteChanged(string name)
        //{

        //}


        //private PropertyDescriptorCollection getCommandInfos()
        //{
        //    List<PropertyDescriptor> list = new List<PropertyDescriptor>();

        //    foreach (string name in properties.Keys)
        //    {
        //        list.Add(new ViewModelPropertyDescriptor(name, properties[name]));
        //    }
        //    return new PropertyDescriptorCollection(list.ToArray());
        //}



        //public AttributeCollection GetAttributes()
        //{
        //    throw new NotImplementedException();
        //}

        //public string GetClassName()
        //{
        //    throw new NotImplementedException();
        //}

        //public string GetComponentName()
        //{
        //    throw new NotImplementedException();
        //}

        //public TypeConverter GetConverter()
        //{
        //    throw new NotImplementedException();
        //}

        //public EventDescriptor GetDefaultEvent()
        //{
        //    throw new NotImplementedException();
        //}

        //public PropertyDescriptor GetDefaultProperty()
        //{
        //    throw new NotImplementedException();
        //}

        //public object GetEditor(Type editorBaseType)
        //{
        //    throw new NotImplementedException();
        //}

        //public EventDescriptorCollection GetEvents(Attribute[] attributes)
        //{
        //    throw new NotImplementedException();
        //}

        //public EventDescriptorCollection GetEvents()
        //{
        //    throw new NotImplementedException();
        //}

        //public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        //{
        //    throw new NotImplementedException();
        //}

        //public virtual PropertyDescriptorCollection GetProperties()
        //{
        //    PropertyDescriptorCollection commands = getCommandInfos();
        //    return commands;
        //}

        //public object GetPropertyOwner(PropertyDescriptor pd)
        //{
        //    throw new NotImplementedException();
        //}

        //private class ViewModelPropertyDescriptor : PropertyDescriptor
        //{
        //    private object value;
        //    public ViewModelPropertyDescriptor(string name, object value)
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
        
        #endregion
    }
    //public class DynamicModelMetaObject : DynamicMetaObject
    //{
    //    private object doValue = null;
    //    public DynamicModelMetaObject(System.Linq.Expressions.Expression parameter, object value)
    //        : base(parameter, BindingRestrictions.Empty, value)
    //    {
    //        this.doValue = value;
    //    }

    //    public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
    //    {
    //        return this.PrintAndReturnIdentity("InvokeMember of method {0}", binder.Name);
    //    }

    //    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    //    {
    //        //return this.PrintAndReturnIdentity("SetMember of property {0}", binder.Name);
    //        //DynamicMetaObject errorSuggestion = binder.FallbackInvokeMember(this, value);
    //        DynamicMetaObject errorSuggestion = binder.FallbackSetMember(this, value);

    //        // run through the plugins and replace our current rule.  Running through
    //        // the list forward means the last plugin has the highest precedence because
    //        // it may throw away the previous rules if it succeeds.
    //        //for (int i = 0; i < Value._plugins.Length; i++)
    //        //{
    //        //    //var pluginDo = DynamicMetaObject.Create(Value._plugins[i],
    //        //    //    Expression.Call(
    //        //    //        typeof(MyDynamicObjectOps).GetMethod("GetPlugin"),
    //        //    //        Expression,
    //        //    //        Expression.Constant(i)
    //        //    //    )
    //        //    //);

    //        //errorSuggestion = binder.FallbackSetMember(this, doValue);
    //        //}

    //        // Do we want DynamicMetaObject to have precedence?  If so then we can do
    //        // one more bind passing what we've produced so far as the rule.  Or if the
    //        // plugins have precedence we could just return the value.  We'll do that
    //        // here based upon the member name.

    //        //if (binder.Name == "Foo")
    //        //{
    //        //    return binder.FallbackInvokeMember(this, args, errorSuggestion);
    //        //}

    //        return errorSuggestion;
    //    }

    //    public override DynamicMetaObject BindConvert(ConvertBinder binder)
    //    {
    //        return base.BindConvert(binder);
    //    }

    //    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    //    {
    //        DynamicMetaObject dmo = this.PrintAndReturnIdentity("GetMember of property {0}", binder.Name);
    //        return dmo;
    //    }

    //    private DynamicMetaObject PrintAndReturnIdentity(string message, string name)
    //    {
    //        //Console.WriteLine(String.Format(message, name));

    //        DynamicMetaObject dmo = new DynamicMetaObject(
    //            Expression,
    //            BindingRestrictions.GetTypeRestriction(
    //                Expression,
    //                typeof(DynamicModelMetaObject)));

    //        return dmo;
    //    }
    //}
}