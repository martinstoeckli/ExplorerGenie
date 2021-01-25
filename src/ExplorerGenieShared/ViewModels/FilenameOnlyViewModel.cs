// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.IO;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// ViewModel for a file path, which contains a full path but only displays the file name.
    /// </summary>
    public class FilenameOnlyViewModel
    {
        /// <summary>
        /// Gets or sets the full file path.
        /// </summary>
        public string FullPath { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Path.GetFileName(FullPath);
        }
    }
}
