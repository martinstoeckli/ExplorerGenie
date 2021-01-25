// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace ExplorerGenieShared.Models
{
    /// <summary>
    /// Model for a user defined goto tool menu.
    /// </summary>
    public class CustomGotoToolModel : IEquatable<CustomGotoToolModel>
    {
        /// <summary>
        /// Gets or sets the title of the menu.
        /// </summary>
        public string MenuTitle { get; set; }

        /// <summary>
        /// Gets or sets the command line string.
        /// </summary>
        public string CommandLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tool should be started as admin or normal.
        /// </summary>
        public bool AsAdmin { get; set; }

        /// <summary>
        /// Creates a deep copy of this instance.
        /// </summary>
        /// <returns>Copy of this instance.</returns>
        public CustomGotoToolModel Clone()
        {
            CustomGotoToolModel result = new CustomGotoToolModel();
            result.MenuTitle = MenuTitle;
            result.CommandLine = CommandLine;
            result.AsAdmin = AsAdmin;
            return result;
        }

        /// <inheritdoc/>
        public override bool Equals(object other)
        {
            return Equals(other as CustomGotoToolModel);
        }

        /// <inheritdoc/>
        public bool Equals(CustomGotoToolModel other)
        {
            if (other == null)
                return false;

            return (MenuTitle == other.MenuTitle)
                && (CommandLine == other.CommandLine)
                && (AsAdmin == other.AsAdmin);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ EqualityComparer<string>.Default.GetHashCode(MenuTitle);
                hashCode = (hashCode * 397) ^ EqualityComparer<string>.Default.GetHashCode(CommandLine);
                hashCode = (hashCode * 397) ^ AsAdmin.GetHashCode();
                return hashCode;
            }
        }
    }

    /// <summary>
    /// List of <see cref="CustomGotoToolModel"/> items.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "List belonging to class.")]
    public class CustomGotoToolModelList : ObservableCollection<CustomGotoToolModel>, IEquatable<CustomGotoToolModelList>
    {
        /// <summary>
        /// Creates a deep copy of this instance.
        /// </summary>
        /// <returns>Copy of this instance.</returns>
        public CustomGotoToolModelList Clone()
        {
            CustomGotoToolModelList result = new CustomGotoToolModelList();
            foreach (CustomGotoToolModel item in this)
            {
                result.Add(item.Clone());
            }
            return result;
        }

        /// <inheritdoc/>
        public override bool Equals(object other)
        {
            return Equals(other as CustomGotoToolModelList);
        }

        /// <inheritdoc/>
        public bool Equals(CustomGotoToolModelList other)
        {
            if (other == null)
                return false;

            bool result = Count == other.Count;
            if (result)
            {
                for (int index = 0; index < Count; index++)
                    result = result && this[index].Equals(other[index]);
            }
            return result;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // include datasource in hash
                int result = 0;
                foreach (var customGotoTool in this)
                    result = (result * 397) ^ customGotoTool.GetHashCode();
                return result;
            }
        }
    }
}
