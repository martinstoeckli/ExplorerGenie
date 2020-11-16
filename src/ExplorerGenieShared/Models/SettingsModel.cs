﻿// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace ExplorerGenieShared.Models
{
    /// <summary>
    /// Model of the applications settings.
    /// </summary>
    public class SettingsModel : IEquatable<SettingsModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsModel"/> class.
        /// </summary>
        public SettingsModel()
        {
            CopyEmailConvertToUnc = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the menu tree CopyFile should be loaded in the
        /// context menu.
        /// </summary>
        public bool CopyFileShowMenu { get; set; }

        /// <summary>
        /// Gets or sets the format for the copy file command.
        /// </summary>
        public CopyFileFormat CopyFileFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only the filename without directory should be
        /// copied, or the whole path.
        /// </summary>
        public bool CopyFileOnlyFilename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a path from a local drive should be converted
        /// to its UNC network path im possible.
        /// </summary>
        public bool CopyFileConvertToUnc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the menu tree CopyEmail should be loaded in the
        /// context menu.
        /// </summary>
        public bool CopyEmailShowMenu { get; set; }

        /// <summary>
        /// Gets or sets the format for the copy email format.
        /// </summary>
        public CopyEmailFormat CopyEmailFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a path from a local drive should be converted
        /// to its UNC network path im possible.
        /// </summary>
        public bool CopyEmailConvertToUnc { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object other)
        {
            return Equals(other as SettingsModel);
        }

        /// <inheritdoc/>
        public bool Equals(SettingsModel other)
        {
            if (other == null)
                return false;

            return (CopyFileShowMenu == other.CopyFileShowMenu)
                && (CopyFileFormat == other.CopyFileFormat)
                && (CopyFileOnlyFilename == other.CopyFileOnlyFilename)
                && (CopyFileConvertToUnc == other.CopyFileConvertToUnc)
                && (CopyEmailShowMenu == other.CopyEmailShowMenu)
                && (CopyEmailFormat == other.CopyEmailFormat)
                && (CopyEmailConvertToUnc == other.CopyEmailConvertToUnc);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // include datasource in hash
                int result = CopyFileShowMenu.GetHashCode();
                result = (result * 397) ^ CopyFileFormat.GetHashCode();
                result = (result * 397) ^ CopyFileOnlyFilename.GetHashCode();
                result = (result * 397) ^ CopyFileConvertToUnc.GetHashCode();
                result = (result * 397) ^ CopyEmailShowMenu.GetHashCode();
                result = (result * 397) ^ CopyEmailFormat.GetHashCode();
                result = (result * 397) ^ CopyEmailConvertToUnc.GetHashCode();
                return result;
            }
        }
    }
}