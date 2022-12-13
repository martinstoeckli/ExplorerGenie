// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action will be called from <see cref="CmdActionSymbolicLink"/> when the application
    /// is restarted with elevated admin privileges. It finishes the creation of a symbolic link.
    /// </summary>
    internal class CmdActionSymbolicLinkElevated : CmdActionBase, ICmdAction
    {
        /// <inheritdoc/>
        public void Execute(List<string> filenames)
        {
            if (filenames.Count != 2)
                return;

            string clickedDirectory = filenames[0];
            string linkTargetDirectory = filenames[1];
            string directoryName = Path.GetFileName(linkTargetDirectory);
            string symbolicLinkDirectory = Path.Combine(clickedDirectory, directoryName);
            try
            {
                WinApi.CreateSymbolicLink(symbolicLinkDirectory, linkTargetDirectory, WinApi.SYMBOLIC_LINK_FLAG.Directory);
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(ex.Message, Language["menuSymbolicLink"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
