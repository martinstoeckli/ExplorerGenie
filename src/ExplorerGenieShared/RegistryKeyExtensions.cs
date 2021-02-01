// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using Microsoft.Win32;

namespace ExplorerGenieShared
{
    /// <summary>
    /// A set of extension methods to the <see cref="RegistryKey"/> class to read values from the
    /// registry.
    /// </summary>
    internal static class RegistryKeyExtensions
    {
        /// <summary>
        /// Reads a value from the registry as type bool.
        /// </summary>
        /// <param name="registry">The registry key which reads the key.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="defaultValue">A default value if no matching value can be found.</param>
        /// <returns>The read value or the default value.</returns>
        public static bool GetValueAsBool(this RegistryKey registry, string name, bool defaultValue)
        {
            object value = registry.GetValue(name, defaultValue, RegistryValueOptions.DoNotExpandEnvironmentNames);
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Reads a value from the registry as type Enum
        /// </summary>
        /// <typeparam name="TEnum">Type of the enum.</typeparam>
        /// <param name="registry">The registry key which reads the key.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="defaultValue">A default value if no matching value can be found.</param>
        /// <returns>The read value or the default value.</returns>
        public static TEnum GetValueAsEnum<TEnum>(this RegistryKey registry, string name, TEnum defaultValue) where TEnum : struct
        {
            object value = registry.GetValue(name, defaultValue, RegistryValueOptions.DoNotExpandEnvironmentNames);
            if (Enum.TryParse(value.ToString(), out TEnum result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Reads a value from the registry as type string.
        /// </summary>
        /// <param name="registry">The registry key which reads the key.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="defaultValue">A default value if no matching value can be found.</param>
        /// <returns>The read value or the default value.</returns>
        public static string GetValueAsString(this RegistryKey registry, string name, string defaultValue)
        {
            object value = registry.GetValue(name, defaultValue, RegistryValueOptions.DoNotExpandEnvironmentNames);
            try
            {
                return value.ToString();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
