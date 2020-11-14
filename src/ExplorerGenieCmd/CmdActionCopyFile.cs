// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Windows.Forms;
using ExplorerGenieShared;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action copies the filename(s) to the Windows clipboard.
    /// </summary>
    internal class CmdActionCopyFile : ICmdAction
    {
        /// <inheritdoc/>
        public void Execute(List<string> filenames)
        {
            const string separator = "\r\n";

            new FilenameSorter().Sort(filenames);
            string clipboardText = string.Join(separator, filenames);
            Clipboard.SetText(clipboardText);
        }
    }
}
