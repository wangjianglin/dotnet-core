using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Lin.Core.Commands
{
    public class RegisterCommandBindings
    {
        private ICommand _command;
        private CanExecuteRoutedEventHandler _CanExecute;
        private ExecutedRoutedEventHandler _Executed;
        private CanExecuteRoutedEventHandler _PreviewCanExecute;
        private ExecutedRoutedEventHandler _PreviewExecuted;


        internal void FireExecute(object sender,ExecutedRoutedEventArgs e)
        {
            _Executed(sender, e);
        }

        internal void FireCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_CanExecute != null)
            {
                _CanExecute(sender, e);
            }
        }

        internal void FirePreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_PreviewCanExecute != null)
            {
                _PreviewCanExecute(sender, e);
            }
        }

        internal void FirePreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_PreviewExecuted != null)
            {
                _PreviewExecuted(sender, e);
            }
        }

        // Events
        public event CanExecuteRoutedEventHandler CanExecute
        {
            add
            {
                CanExecuteRoutedEventHandler handler;
                CanExecuteRoutedEventHandler canExecute = this._CanExecute;
                do
                {
                    handler = canExecute;
                    CanExecuteRoutedEventHandler handler3 = (CanExecuteRoutedEventHandler)Delegate.Combine(handler, value);
                    canExecute = Interlocked.CompareExchange<CanExecuteRoutedEventHandler>(ref this._CanExecute, handler3, handler);
                }
                while (canExecute != handler);
            }
            remove
            {
                CanExecuteRoutedEventHandler handler;
                CanExecuteRoutedEventHandler canExecute = this._CanExecute;
                do
                {
                    handler = canExecute;
                    CanExecuteRoutedEventHandler handler3 = (CanExecuteRoutedEventHandler)Delegate.Remove(handler, value);
                    canExecute = Interlocked.CompareExchange<CanExecuteRoutedEventHandler>(ref this._CanExecute, handler3, handler);
                }
                while (canExecute != handler);
            }
        }

        public event ExecutedRoutedEventHandler Executed
        {
            add
            {
                ExecutedRoutedEventHandler handler;
                ExecutedRoutedEventHandler executed = this._Executed;
                do
                {
                    handler = executed;
                    ExecutedRoutedEventHandler handler3 = (ExecutedRoutedEventHandler)Delegate.Combine(handler, value);
                    executed = Interlocked.CompareExchange<ExecutedRoutedEventHandler>(ref this._Executed, handler3, handler);
                }
                while (executed != handler);
            }
            remove
            {
                ExecutedRoutedEventHandler handler;
                ExecutedRoutedEventHandler executed = this._Executed;
                do
                {
                    handler = executed;
                    ExecutedRoutedEventHandler handler3 = (ExecutedRoutedEventHandler)Delegate.Remove(handler, value);
                    executed = Interlocked.CompareExchange<ExecutedRoutedEventHandler>(ref this._Executed, handler3, handler);
                }
                while (executed != handler);
            }
        }

        public event CanExecuteRoutedEventHandler PreviewCanExecute
        {
            add
            {
                CanExecuteRoutedEventHandler handler;
                CanExecuteRoutedEventHandler previewCanExecute = this._PreviewCanExecute;
                do
                {
                    handler = previewCanExecute;
                    CanExecuteRoutedEventHandler handler3 = (CanExecuteRoutedEventHandler)Delegate.Combine(handler, value);
                    previewCanExecute = Interlocked.CompareExchange<CanExecuteRoutedEventHandler>(ref this._PreviewCanExecute, handler3, handler);
                }
                while (previewCanExecute != handler);
            }
            remove
            {
                CanExecuteRoutedEventHandler handler;
                CanExecuteRoutedEventHandler previewCanExecute = this._PreviewCanExecute;
                do
                {
                    handler = previewCanExecute;
                    CanExecuteRoutedEventHandler handler3 = (CanExecuteRoutedEventHandler)Delegate.Remove(handler, value);
                    previewCanExecute = Interlocked.CompareExchange<CanExecuteRoutedEventHandler>(ref this._PreviewCanExecute, handler3, handler);
                }
                while (previewCanExecute != handler);
            }
        }

        public event ExecutedRoutedEventHandler PreviewExecuted
        {
            add
            {
                ExecutedRoutedEventHandler handler;
                ExecutedRoutedEventHandler previewExecuted = this._PreviewExecuted;
                do
                {
                    handler = previewExecuted;
                    ExecutedRoutedEventHandler handler3 = (ExecutedRoutedEventHandler)Delegate.Combine(handler, value);
                    previewExecuted = Interlocked.CompareExchange<ExecutedRoutedEventHandler>(ref this._PreviewExecuted, handler3, handler);
                }
                while (previewExecuted != handler);
            }
            remove
            {
                ExecutedRoutedEventHandler handler;
                ExecutedRoutedEventHandler previewExecuted = this._PreviewExecuted;
                do
                {
                    handler = previewExecuted;
                    ExecutedRoutedEventHandler handler3 = (ExecutedRoutedEventHandler)Delegate.Remove(handler, value);
                    previewExecuted = Interlocked.CompareExchange<ExecutedRoutedEventHandler>(ref this._PreviewExecuted, handler3, handler);
                }
                while (previewExecuted != handler);
            }
        }

        // Methods
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public RegisterCommandBindings()
        {
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public RegisterCommandBindings(ICommand command)
            : this(command, null, null)
        {
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public RegisterCommandBindings(ICommand command, ExecutedRoutedEventHandler executed)
            : this(command, executed, null)
        {
        }

        public RegisterCommandBindings(ICommand command, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            this._command = command;
            if (executed != null)
            {
                this.Executed += executed;
            }
            if (canExecute != null)
            {
                this.CanExecute += canExecute;
            }
        }

        internal void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.RoutedEvent == CommandManager.CanExecuteEvent)
                {
                    if (this._CanExecute == null)
                    {
                        if (!e.CanExecute && (this._Executed != null))
                        {
                            e.CanExecute = true;
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        this._CanExecute(sender, e);
                        if (e.CanExecute)
                        {
                            e.Handled = true;
                        }
                    }
                }
                else if (this._PreviewCanExecute != null)
                {
                    this._PreviewCanExecute(sender, e);
                    if (e.CanExecute)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand Command
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._command;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this._command = value;
            }
        }
    }
}
