// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace ExplorerGenieShared.Models
{
    /// <summary>
    /// Model for a user defined goto tool menu.
    /// </summary>
    [XmlType("custom_goto_tools")]
    public class CustomGotoToolModel : IEquatable<CustomGotoToolModel>
    {
        /// <summary>
        /// Gets or sets the title of the menu.
        /// </summary>
        [XmlAttribute("title")]
        public string MenuTitle { get; set; }

        /// <summary>
        /// Gets or sets the command line string.
        /// </summary>
        [XmlAttribute("command")]
        public string CommandLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tool should be started as admin or normal.
        /// </summary>
        [XmlAttribute("admin")]
        public bool AsAdmin { get; set; }

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
    [XmlType("custom_goto_tools")]
    [XmlRoot("custom_goto_tools")]
    public class CustomGotoToolModelList : ObservableCollection<CustomGotoToolModel>, IEquatable<CustomGotoToolModelList>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGotoToolModelList"/> class.
        /// </summary>
        public CustomGotoToolModelList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGotoToolModelList"/> class
        /// that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public CustomGotoToolModelList(IEnumerable<CustomGotoToolModel> collection)
            : base(collection)
        {
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
