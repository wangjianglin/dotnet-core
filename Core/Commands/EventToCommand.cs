using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Windows.Interactivity;
using System.Windows.Input;
using control = System.Windows.Controls;
using System.Windows.Controls;

namespace Lin.Core.Commands
{
    //class EventToCommand
    //{
    //}
    public class EventToCommand : TriggerAction<FrameworkElement>
    {
        // Fields
        private object _commandParameterValue;
        private bool? _mustToggleValue;
        [CompilerGenerated]
        private bool k__BackingField;
        public static readonly DependencyProperty CommandParameterProperty;
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty MustToggleIsEnabledProperty;

        // Methods
        static EventToCommand()
        {
            CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventToCommand), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
            {
                EventToCommand command = s as EventToCommand;
                if ((command != null) && (command.AssociatedObject != null))
                {
                    command.EnableDisableElement();
                }
            }));
            CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
            {
                OnCommandChanged(s as EventToCommand, e);
            }));
            MustToggleIsEnabledProperty = DependencyProperty.Register("MustToggleIsEnabled", typeof(bool), typeof(EventToCommand), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
            {
                EventToCommand command = s as EventToCommand;
                if ((command != null) && (command.AssociatedObject != null))
                {
                    command.EnableDisableElement();
                }
            }));
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
            this.Invoke(null);
        }

        protected override void Invoke(object parameter)
        {
            if (!this.AssociatedElementIsDisabled())
            {
                ICommand command = this.GetCommand();
                object commandParameterValue = this.CommandParameterValue;
                if ((commandParameterValue == null) && this.PassEventArgsToCommand)
                {
                    commandParameterValue = parameter;
                }
                //if (command is RoutedCommand)
                //{
                //    RoutedCommand tmp = command as RoutedCommand;
                //    if (this.GetAssociatedObject().Parent is ContextMenu)
                //    {
                //        ContextMenu cm = this.GetAssociatedObject().Parent as ContextMenu;
                //        UIElement tmpTarget = cm.PlacementTarget;
                //        tmp.Execute(commandParameterValue, tmpTarget);
                //        return;
                //    }
                //    //if(((System.Windows.Controls.ContextMenu)this.GetAssociatedObject().Parent).PlacementTarget
                //}
                if ((command != null) && command.CanExecute(commandParameterValue))
                {
                    command.Execute(commandParameterValue);
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.EnableDisableElement();
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.EnableDisableElement();
        }

        private static void OnCommandChanged(EventToCommand element, DependencyPropertyChangedEventArgs e)
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
