using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using control = System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Runtime.CompilerServices;

namespace Lin.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CommandBehavior<T> : Behavior<T> where T : System.Windows.DependencyObject
    {

        private object _commandParameterValue;
        private bool? _mustToggleValue;
        private bool k__BackingField;


        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandBehavior<T>), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            CommandBehavior<T> command = s as CommandBehavior<T>;
            if ((command != null) && (command.AssociatedObject != null))
            {
                command.EnableDisableElement();
            }
        }));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandBehavior<T>), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            OnCommandChanged(s as CommandBehavior<T>, e);
        }));

        public static readonly DependencyProperty MustToggleIsEnabledProperty = DependencyProperty.Register("MustToggleIsEnabled", typeof(bool), typeof(CommandBehavior<T>), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            CommandBehavior<T> command = s as CommandBehavior<T>;
            if ((command != null) && (command.AssociatedObject != null))
            {
                command.EnableDisableElement();
            }
        }));


        private control.Control GetAssociatedObject()
        {
            return (base.AssociatedObject as Control);
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.EnableDisableElement();
        }
        private static void OnCommandChanged(CommandBehavior<T> element, DependencyPropertyChangedEventArgs e)
        {
            if (element != null)
            {
                if (e.OldValue != null)
                {
                    ((ICommand)e.OldValue).CanExecuteChanged -= new EventHandler(element.OnCommandCanExecuteChanged);
                }
                ICommand newValue = (ICommand)e.NewValue;
                if (newValue != null)
                {
                    newValue.CanExecuteChanged += new EventHandler(element.OnCommandCanExecuteChanged);
                }
                element.EnableDisableElement();
            }
        }



        public ICommand Command
        {
            get
            {
                return (ICommand)base.GetValue(CommandProperty);
            }
            set
            {
                base.SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return base.GetValue(CommandParameterProperty);
            }
            set
            {
                base.SetValue(CommandParameterProperty, value);
            }
        }

        private object CommandParameterValue
        {
            get
            {
                return (this._commandParameterValue ?? this.CommandParameter);
            }
            set
            {
                this._commandParameterValue = value;
                this.EnableDisableElement();
            }
        }

        public bool MustToggleIsEnabled
        {
            get
            {
                return (bool)base.GetValue(MustToggleIsEnabledProperty);
            }
            set
            {
                base.SetValue(MustToggleIsEnabledProperty, value);
            }
        }

        private bool MustToggleIsEnabledValue
        {
            get
            {
                return (!this._mustToggleValue.HasValue ? this.MustToggleIsEnabled : this._mustToggleValue.Value);
            }
            set
            {
                this._mustToggleValue = new bool?(value);
                this.EnableDisableElement();
            }
        }

        public bool PassEventArgsToCommand
        {
            [CompilerGenerated]
            get
            {
                return this.k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.k__BackingField = value;
            }
        }

        private void EnableDisableElement()
        {
            Control associatedObject = this.GetAssociatedObject();
            if (associatedObject != null)
            {
                ICommand command = this.Command;
                if (this.MustToggleIsEnabledValue && (command != null))
                {
                    associatedObject.IsEnabled = command.CanExecute(this.CommandParameterValue);
                }
            }
        }


        protected virtual bool CanInvoke(object parameter)
        {
            return true;
        }

        protected void Invoke(object parameter)
        {
            if (!this.AssociatedElementIsDisabled() && this.CanInvoke(parameter))
            {
                

                ICommand command = this.Command;
                object commandParameterValue = this.CommandParameterValue;
                if ((commandParameterValue == null) && this.PassEventArgsToCommand)
                {
                    commandParameterValue = parameter;
                }
                if ((command != null) && command.CanExecute(commandParameterValue))
                {
                    command.Execute(commandParameterValue);
                }
            }
        }


        private bool AssociatedElementIsDisabled()
        {
            control.Control associatedObject = this.GetAssociatedObject();
            return ((associatedObject != null) && !associatedObject.IsEnabled);
        }
    }
}
