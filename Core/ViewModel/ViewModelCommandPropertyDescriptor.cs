using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lin.Core.ViewModel
{
    public class ViewModelCommandPropertyDescriptor : PropertyDescriptor
    {
        private ViewModelBase vm;
        private string name;
        private MethodInfo method;
        private ICommand commmand = null;
        public ViewModelCommandPropertyDescriptor(ViewModelBase vm, string name, MethodInfo method, MethodInfo canExecuteMethod)
           : base(name, null)
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
            this.vm = vm;
            this.name = name;
            this.method = method;
            commmand = new Lin.Core.Commands.ReplayCommand(obj =>
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
        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override Type ComponentType
        {
            get { throw new NotImplementedException(); }
        }

        public override object GetValue(object component)
        {
            return commmand;
        }

        public override bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public override Type PropertyType
        {
            get { return typeof(ICommand); }
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }
    }
}
