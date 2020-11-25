// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
    /// and to control access.
    /// </summary>
    public class RelayCommand : RelayCommand<object>, ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute)
            : base(execute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute logic when the command is called.</param>
        /// <param name="canExecute">The execution status logic (enabled/disabled).</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute)
        {
        }
    }

    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
    /// </summary>
    /// <typeparam name="T">Type of the parameter, the command can pass on to its action.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Generic variation.")]
    #pragma warning disable IDE0034 // I like an explicit 'default' expression
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute = null;
        private readonly Predicate<T> _canExecute = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
        /// This is just a wrapper around the MvvmLight relay command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="execute">The execute logic when the command is called.</param>
        /// <param name="canExecute">The execution status logic (enabled/disabled).</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand Members

        /// <inheritdoc/>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute((T)parameter);
            else
                return true;
        }

        /// <inheritdoc/>
        /// <summary>Implementation for the WPF platform.</summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <inheritdoc/>
        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                if (parameter is T typedParameter)
                    _execute(typedParameter);
                else
                    _execute(default(T));
            }
        }

        #endregion // ICommand Members
    }
}
