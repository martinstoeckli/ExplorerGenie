// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// The opposite to the  the <see cref="System.Windows.Controls.BooleanToVisibilityConverter"/>.
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converter for XAML binding, which can connect the visibility property with a viewmodel
        /// property of type bool (visible or hidden).
        /// <example><code>
        /// ≺Window.Resources≻
        ///   ≺vm:InverseBooleanToVisibilityConverter x:Key="inverseBoolToVisibility" /≻
        /// ≺/Window.Resources≻
        /// <Image Visibility="{Binding IsHidden, Converter={StaticResource inverseBoolToVisibility}}"/>
        /// </code></example>
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Code adapted from BoolToVisibilityConverter
            bool flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is bool?)
            {
                bool? flag2 = (bool?)value;
                flag = flag2.HasValue && flag2.Value;
            }

            return (flag) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts the <paramref name="parameter"/> of type visibility to a bool type.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value != Visibility.Visible;
            }

            return true;
        }
    }
}
