// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Globalization;
using System.Windows.Data;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// Converter for XAML binding, which can connect a radio button with a viewmodel property of
    /// type Enum.
    /// <example><code>
    /// ≺Window.Resources≻
    ///   ≺vm:RadioEnumConverter x:Key="radioEnumConverter" /≻
    /// ≺/Window.Resources≻
    /// ≺RadioButton IsChecked="{Binding Path=MyWeekdaysProperty, Converter={StaticResource radioEnumConverter}, ConverterParameter=Sunday}"≻A sunny day≺/RadioButton≻
    /// </code></example>
    /// </summary>
    public class RadioEnumConverter : IValueConverter
    {
        /// <summary>
        /// Checks whether the <paramref name="parameter"/> matches the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Current value of an enumeration property of the viewmodel.</param>
        /// <param name="targetType">Unused.</param>
        /// <param name="parameter">Enum value which is defined in the XAML attribute 'ConverterParameter'
        /// of the radio button. If it matches with <paramref name="value"/>, this radio button should
        /// be checked.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>Returns true if the radio button should be checked, otherwise false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Enum))
                throw new ArgumentOutOfRangeException(nameof(value), "The bound property in the viewmodel must be an Enum type.");

            try
            {
                object parameterValue = Enum.Parse(value.GetType(), parameter.ToString());
                return parameterValue.Equals(value);
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException(string.Format("The attribute 'ConverterParameter' defined in the XAML, must be the string representation of a member of the '{0}' enumeration.", value.GetType().Name), ex);
            }
        }

        /// <summary>
        /// Converts the <paramref name="parameter"/> to the Enum whose type is passed in <paramref name="targetType"/>.
        /// </summary>
        /// <param name="value">True for a checked radiobutton, false for an unchecked radiobutton.</param>
        /// <param name="targetType">Type of the enumeration, used in the property of the viewmodel.</param>
        /// <param name="parameter">Enum value which is defined in the XAML attribute 'ConverterParameter'
        /// of the radio button.</param>
        /// <param name="culture">Unused.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter?.ToString();

            try
            {
                return Enum.Parse(targetType, parameterString);
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException(string.Format("The attribute 'ConverterParameter' defined in the XAML, must be the string representation of a member of the '{0}' enumeration.", targetType.Name), ex);
            }
        }
    }
}
