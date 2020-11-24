// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Extension methods for the <see cref="List<>"/> class.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// List extension, which applies a modification to each element of the list.
        /// </summary>
        /// <typeparam name="T">Type of the list.</typeparam>
        /// <param name="elements">The list to modify.</param>
        /// <param name="modificator">Function which will be called for each element of the list.</param>
        public static void ModifyEach<T>(this IList<T> elements, Func<T, T> modificator)
        {
            for (int index = 0; index < elements.Count; index++)
            {
                T element = elements[index];
                elements[index] = modificator(element);
            }
        }
    }
}
