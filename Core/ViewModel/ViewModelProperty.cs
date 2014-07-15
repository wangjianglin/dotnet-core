using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Lin.Core.ViewModel
{
    [Serializable]
    public class ViewModelProperty
    { 
        public event PropertyChangedEventHandler PropertyChanged;
        public ViewModelProperty(){
        }

        /// <summary>
        /// 得到所有的属性名称集合
        /// </summary>
        public string[] Names
        {
            get
            {
                if (_Commands != null)
                {
                    return _Commands.Keys.ToArray();
                }
                return new string[] { };
            }
        }

        private Dictionary<string, object> _Commands = new Dictionary<string, object>();
        public object this[string s]
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
                return null;
            }
            set
            {
                this.AddValue(s, value);
            }
        }
        private void AddValue(string commandName, object command)
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
        internal void IintValue(string commandName, object command)
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
