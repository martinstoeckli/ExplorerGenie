// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action will be called from <see cref="CmdActionSymbolicLink"/> when the application
    /// is restarted with elevated admin privileges. It finishes the creation of a symbolic link.
    /// </summary>
    internal class CmdActionSymbolicLinkElevated : ICmdAction
    {
        /// <inheritdoc/>
        public void Execute(List<string> filenames)
        {
            if (filenames.Count != 2)
                return;

            string clickedDirectory = filenames[0];
            string childDirectory = filenames[1];
            string lastChildDirectoryPart = Path.GetFileName(childDirectory);
            string parentDirectory = Path.Combine(clickedDirectory, lastChildDirectoryPart);
            try
            {
                CreateSymbolicLink(parentDirectory, childDirectory, SYMBOLIC_LINK_FLAG.Directory);
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(ex.Message, "err", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        [Flags]
        enum SYMBOLIC_LINK_FLAG
        {
            File = 0,
            Directory = 1,
            AllowUnprivilegedCreate = 2
        }

        private static void CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SYMBOLIC_LINK_FLAG dwFlags)
        {
            bool success = CreateSymbolicLinkInternal(lpSymlinkFileName, lpTargetFileName, dwFlags);
            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        [DllImport("Kernel32.dll", EntryPoint = "CreateSymbolicLinkW", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool CreateSymbolicLinkInternal(string lpSymlinkFileName, string lpTargetFileName, SYMBOLIC_LINK_FLAG dwFlags);
    }
}
