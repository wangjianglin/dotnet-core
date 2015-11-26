using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Lin.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class ReplayCommand : System.Windows.DependencyObject,ICommand 
    {
        public static readonly DependencyProperty IsRestProperty = DependencyProperty.RegisterAttached("IsRest", typeof(bool?), typeof(ReplayCommand),
            new PropertyMetadata(null, (DependencyObject dc, DependencyPropertyChangedEventArgs args) =>
            {
                if (((ReplayCommand)dc).CanExecuteChanged != null)
                {
                    ((ReplayCommand)dc).CanExecuteChanged(dc, new EventArgs());
                }
            }));
        private bool? IsRest
        {
            get { return (bool?)this.GetValue(IsRestProperty); }
            set { this.SetValue(IsRestProperty, value); }
        }

        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;     
        #endregion   
        #region Constructors 
        public ReplayCommand(Action<object> execute)
            : this(execute, null)
        {
        } 
        public ReplayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;           
        } 
        #endregion   
        #region ICommand Members 
        //[DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (IsRest == null && _canExecute == null)
            {
                return true;
            }
            if (IsRest != null && _canExecute != null)
            {
                return (bool)IsRest && _canExecute(parameter);
            }
            if (IsRest != null)
            {
                return (bool)IsRest;
            }
            if (_canExecute != null)
            {
                return _canExecute(parameter);
            }
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void FireCanExecute()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged; 
        public event Action<object> PreExecute;
        public event Action<object> Executed;
        public void Execute(object parameter)
        {
            if (PreExecute != null)
            {
                PreExecute(parameter);
            }
            _execute(parameter);
            if (Executed != null)
            {
                Executed(parameter);
            }
        }

        #endregion  
    }
}