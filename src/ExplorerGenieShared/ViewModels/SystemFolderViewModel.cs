// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// ViewModel for a single system folder.
    /// </summary>
    public class SystemFolderViewModel
    {
        /// <summary>
        /// Gets or sets the name of the system folder <see cref="Environment.SpecialFolder"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the full path of the system folder.
        /// </summary>
        public string Path { get; set; }
    }
}
