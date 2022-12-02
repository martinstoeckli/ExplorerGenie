// Copyright © 2020 Martin Stoeckli.
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
        private CustomGotoToolModelList _customGotoTools;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsModel"/> class.
        /// </summary>
        public SettingsModel()
        {
            FreshInstallation = true;
            CopyFileShowMenu = true;
            CopyEmailConvertToUnc = true; // receiver probably won't have access to local drives.
            GotoShowMenu = true;
            GotoCommandPrompt = true;
            GotoPowerShell = true;
            GotoExplorer = true;
            HashShowMenu = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether ExplorerGenie is shown the first time after
        /// an installation.
        /// </summary>
        public bool FreshInstallation { get; set; }

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
        /// Gets or sets the format for the copy email format.
        /// </summary>
        public CopyEmailFormat CopyEmailFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a path from a local drive should be converted
        /// to its UNC network path im possible.
        /// </summary>
        public bool CopyEmailConvertToUnc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the menu tree CopyEmail should be loaded in the
        /// context menu.
        /// </summary>
        public bool GotoShowMenu { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the "go to command prompt" menu should be shown.
        /// </summary>
        public bool GotoCommandPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the "go to power shell" menu should be shown.
        /// </summary>
        public bool GotoPowerShell { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the "go to explorer" menu should be shown.
        /// </summary>
        public bool GotoExplorer { get; set; }

        /// <summary>
        /// Gets or sets a list of custom goto tools.
        /// </summary>
        public CustomGotoToolModelList CustomGotoTools
        {
            get { return _customGotoTools ?? (_customGotoTools = new CustomGotoToolModelList()); }
            set { _customGotoTools = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the menu tree "hash" should be loaded in the
        /// context menu.
        /// </summary>
        public bool HashShowMenu { get; set; }

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

            return (FreshInstallation == other.FreshInstallation)
                && (CopyFileShowMenu == other.CopyFileShowMenu)
                && (CopyFileFormat == other.CopyFileFormat)
                && (CopyFileOnlyFilename == other.CopyFileOnlyFilename)
                && (CopyFileConvertToUnc == other.CopyFileConvertToUnc)
                && (CopyEmailFormat == other.CopyEmailFormat)
                && (CopyEmailConvertToUnc == other.CopyEmailConvertToUnc)
                && (GotoShowMenu == other.GotoShowMenu)
                && (GotoCommandPrompt == other.GotoCommandPrompt)
                && (GotoPowerShell == other.GotoPowerShell)
                && (GotoExplorer == other.GotoExplorer)
                && (HashShowMenu == other.HashShowMenu)
                && (CustomGotoTools.Equals(other.CustomGotoTools));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // include datasource in hash
                int result = FreshInstallation.GetHashCode();
                result = (result * 397) ^ CopyFileShowMenu.GetHashCode();
                result = (result * 397) ^ CopyFileFormat.GetHashCode();
                result = (result * 397) ^ CopyFileOnlyFilename.GetHashCode();
                result = (result * 397) ^ CopyFileConvertToUnc.GetHashCode();
                result = (result * 397) ^ CopyEmailFormat.GetHashCode();
                result = (result * 397) ^ CopyEmailConvertToUnc.GetHashCode();
                result = (result * 397) ^ GotoShowMenu.GetHashCode();
                result = (result * 397) ^ GotoCommandPrompt.GetHashCode();
                result = (result * 397) ^ GotoPowerShell.GetHashCode();
                result = (result * 397) ^ GotoExplorer.GetHashCode();
                result = (result * 397) ^ HashShowMenu.GetHashCode();
                result = (result * 397) ^ CustomGotoTools.GetHashCode();
                return result;
            }
        }
    }
}
