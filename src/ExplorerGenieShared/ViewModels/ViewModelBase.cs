// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// Base class for all other view models.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether modifications where done to the
        /// view model. This property is automatically updated when calling <see cref="SetProperty{T}(ref T, T, bool, string)"/>.
        /// </summary>
        public bool Modified { get; set; }

        /// <summary>
        /// This event can be consumed by a view, to find out when a certain property has changed,
        /// so it can be updated.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event, to inform the view of changes.
        /// Calling this method is usually not necessary, because the method <see cref="SetProperty{T}(ref T, T, bool, string)"/>
        /// will call it automatically.
        /// </summary>
        /// <param name="propertyName">Name of the property which has changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Assigns a new value to the property and raises the PropertyChanged event if necessary.
        /// </summary>
        /// <typeparam name="T">The type of the property to change.</typeparam>
        /// <param name="propertyMember">The backing field in the viewmodel.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="updateModified">Value indicating whether the <see cref="Modified"/> flag
        /// should be updated if the value changes.</param>
        /// <param name="propertyName">(optional) The name of the property to change.</param>
        /// <returns>Returns true if the PropertyChanged event was raised, otherwise false.</returns>
        protected bool SetProperty<T>(ref T propertyMember, T newValue, bool updateModified, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(propertyMember, newValue))
                return false;

            propertyMember = newValue;
            OnPropertyChanged(propertyName);
            if (updateModified)
                Modified = true;
            return true;
        }

        /// <summary>
        /// Assigns a new value to the property and raises the PropertyChanged event if necessary.
        /// </summary>
        /// <typeparam name="T">The type of the property to change.</typeparam>
        /// <param name="getterFromModel">Gets the value of the property from the model.</param>
        /// <param name="setterToModel">Sets the value of the property to the model.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="updateModified">Value indicating whether the <see cref="Modified"/></param>
        /// should be updated if the value changes.
        /// <param name="propertyName">(optional) The name of the property to change.</param>
        /// <returns>Returns true if the PropertyChanged event was raised, otherwise false.</returns>
        protected bool SetPropertyIndirect<T>(Func<T> getterFromModel, Action<T> setterToModel, T newValue, bool updateModified, [CallerMemberName] string propertyName = null)
        {
            T oldValue = getterFromModel();

            // If both values are equal, then do not change anything
            if ((oldValue == null) && (newValue == null))
                return false;
            if ((oldValue != null) && oldValue.Equals(newValue))
                return false;

            setterToModel(newValue);
            OnPropertyChanged(propertyName);
            if (updateModified)
                Modified = true;
            return true;
        }
    }
}
