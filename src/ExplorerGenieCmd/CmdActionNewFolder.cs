// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action can add a new folder to the selected directory.
    /// </summary>
    internal class CmdActionNewFolder : ICmdAction
    {
        /// <inheritdoc/>
        public void Execute(List<string> filenames)
        {
            // Not yet decided whether this action will be implemented
        }
    }
}
