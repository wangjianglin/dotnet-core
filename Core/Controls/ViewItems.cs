using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;

namespace Lin.Core.Controls
{
    public class ViewItems : System.Windows.Controls.ItemsControl, System.ComponentModel.INotifyPropertyChanged
    {
        private Dictionary<string, object> map = null;
        public ViewItems()
        {
        }
        public DependencyObject AttachedObject { get; internal set; }
        private void ProcessItems()
        {
            map = new Dictionary<string, object>();
            if (base.Items != null && base.Items.Count > 0)
            {
                List<object> list = new List<object>();
                foreach (object item in base.Items)
                {
                    list.Add(item);
                }

                foreach (object item in list)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    base.Items.Remove(item);
                    FrameworkElement element = item as FrameworkElement;
                    if (element != null && element.Name != null && element.Name != "")
                    {
                        if (!map.ContainsKey(element.Name))
                        {
                            map.Add(element.Name, element);
                        }
                    }
                    else
                    {
                        Type type = item.GetType();
                        PropertyInfo property = type.GetProperty("Name");
                        if (property != null)
                        {
                            string propertyValue = property.GetValue(item, null) as string;
                            if (propertyValue != null && propertyValue != "")
                            {
                                if (!map.ContainsKey(element.Name))
                                {
                                    map.Add(element.Name, element);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreatMap()
        {
            if (map == null)
            {
                lock (lockObject)
                {
                    if (map == null)
                    {
                        ProcessItems();
                    }
                }
            }
        }

        private object lockObject = new object();
        public object this[string name]
        {
            get
            {
                CreatMap();
                if (name == null || name == "")
                {
                    return null;
                }
                if (map.ContainsKey(name))
                {
                    return map[name];
                }

                if (!map.ContainsKey(name))
                {
                    lock (lockObject)
                    {
                        if (!map.ContainsKey(name))
                        {
                            FindNameFromINameScope(name);
                        }
                    }
                }

                if (map.ContainsKey(name))
                {
                    return map[name];
                }
                return null;
            }
            set
            {
                CreatMap();
                if (String.IsNullOrEmpty(name))
                {
                    return;
                }
                if (value == null)
                {
                    if (map.ContainsKey(name))
                    {
                        map.Remove(name);
                    }
                }
                else
                {
                    if (map.ContainsKey(name))
                    {
                        map[name] = value;
                    }
                    else
                    {
                        map.Add(name, value);
                    }
                }
                this.OnPropertyChanged("");
            }
        }
        private object FindNameFromINameScope(string name)
        {
            if (this.AttachedObject == null)
            {
                return null;
            }
            INameScope scope = this.AttachedObject.GetValue(NameScope.NameScopeProperty) as INameScope;

            if (scope == null)
            {
                return null;
            }


            object tmpObject = scope.FindName(name);

            if (tmpObject != null)
            {
                this.Items.Remove(tmpObject);
            }
            if (!map.ContainsKey(name))
            {
                map.Add(name, tmpObject);
            }
            return tmpObject;
        }


        public void VerifyPropertyName(string propertyName)
        {
        }

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
        #endregion
    }
}