using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using control = System.Windows.Controls;
using System.Windows.Data;

namespace AD.Core.Commands
{
    public class PropertyChangedToCommand : TriggerAction<FrameworkElement>
    {
       // Fields
        private object _commandParameterValue;
        private bool? _mustToggleValue;
        [CompilerGenerated]
        private bool k__BackingField;
        public static readonly DependencyProperty CommandParameterProperty;
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty MustToggleIsEnabledProperty;
        public static readonly DependencyProperty PropertyProperty;
        public static readonly DependencyProperty ValueProperty;

        // Methods
        static PropertyChangedToCommand()
        {
            CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(PropertyChangedToCommand), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
            {
                PropertyChangedToCommand command = s as PropertyChangedToCommand;
                if ((command != null) && (command.AssociatedObject != null))
                {
                    command.EnableDisableElement();
                }
            }));
            PropertyProperty = DependencyProperty.Register("Property", typeof(string), typeof(PropertyChangedToCommand), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
            {
                PropertyChangedToCommand command = s as PropertyChangedToCommand;
                if ((command != null) && (command.AssociatedObject != null))
                {
                    command.OnPropertyPropertyChanged(e);
                }
            }));
            ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(PropertyChangedToCommand), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
            {
                PropertyChangedToCommand command = s as PropertyChangedToCommand;
                if ((command != null) && (command.AssociatedObject != null))
                {
                    command.Value = e.NewValue;
                }
            }));
            CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(PropertyChangedToCommand), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
            {
                OnCommandChanged(s as PropertyChangedToCommand, e);
            }));
            MustToggleIsEnabledProperty = DependencyProperty.Register("MustToggleIsEnabled", typeof(bool), typeof(PropertyChangedToCommand), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
            {
                PropertyChangedToCommand command = s as PropertyChangedToCommand;
                if ((command != null) && (command.AssociatedObject != null))
                {
                    command.EnableDisableElement();
                }
            }));
        }
        private void OnPropertyPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            this.Property = args.NewValue as string;
            ReBinding();
        }
        private static readonly DependencyProperty PropertyChangedProperty = DependencyProperty.Register("PropertyChange", typeof(object), typeof(PropertyChangedToCommand), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs args)
            {
                //OnCommandChanged(s as TriggerActionToCommand, e);
                //MessageBox.Show("success!");
                PropertyChangedToCommand command = s as PropertyChangedToCommand;
                if ((command != null) && (command.AssociatedObject != null))
                {
                    bool result=false;
                    if (args.NewValue == null && command.Value == null)
                    {
                        result = true;
                    }
                    else if (args.NewValue != null && command.Value != null)
                    {
                        if (args.NewValue.GetType() == command.Value.GetType())
                        {
                            result = args.NewValue.Equals(command.Value);
                        }
                        else
                        {
                            result = args.NewValue.ToString().Equals(command.Value.ToString());
                        }
                    }
                    if (!command.AssociatedElementIsDisabled() && result)
                    {
                        ICommand icommand = command.GetCommand();
                        object commandParameterValue = command.CommandParameterValue;
                        if ((commandParameterValue == null) && command.PassEventArgsToCommand)
                        {
                            commandParameterValue = null;
                        }
                        //if ((icommand != null) && icommand.CanExecute(commandParameterValue))
                        if (icommand != null)
                        {
                            icommand.Execute(commandParameterValue);
                        }
                    }
                }
            }));
        private void ReBinding()
        {
            if (this.GetAssociatedObject() != null && this.Property != null && this.Property != "")
            {
                Binding binding = new Binding(this.Property);
                binding.Source = this.GetAssociatedObject();
                BindingOperations.SetBinding(this, PropertyChangedProperty, binding);
            }
        }
        public string Property
        {
            get { return (string)this.GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }
        public object Value
        {
            get { return (object)this.GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        private bool AssociatedElementIsDisabled()
        {
            control.Control associatedObject = this.GetAssociatedObject();
            return ((associatedObject != null) && !associatedObject.IsEnabled);
        }

        private void EnableDisableElement()
        {
            control.Control associatedObject = this.GetAssociatedObject();
            if (associatedObject != null)
            {
                ICommand command = this.GetCommand();
                if (this.MustToggleIsEnabledValue && (command != null))
                {
                    associatedObject.IsEnabled = command.CanExecute(this.CommandParameterValue);
                }
            }
        }

        private control.Control GetAssociatedObject()
        {
            return (base.AssociatedObject as control.Control);
        }

        private ICommand GetCommand()
        {
            return this.Command;
        }

        public void Invoke()
        {
            //this.Invoke(null);
        }

        protected override void Invoke(object parameter)
        {
            //if (!this.AssociatedElementIsDisabled())
            //{
            //    ICommand command = this.GetCommand();
            //    object commandParameterValue = this.CommandParameterValue;
            //    if ((commandParameterValue == null) && this.PassEventArgsToCommand)
            //    {
            //        commandParameterValue = parameter;
            //    }
            //    if ((command != null) && command.CanExecute(commandParameterValue))
            //    {
            //        command.Execute(commandParameterValue);
            //    }
            //}
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.EnableDisableElement();
            ReBinding();
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.EnableDisableElement();
        }

        private static void OnCommandChanged(PropertyChangedToCommand element, DependencyPropertyChangedEventArgs e)
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

        // Properties
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

        public object CommandParameterValue
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

        public bool MustToggleIsEnabledValue
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
    }

}
