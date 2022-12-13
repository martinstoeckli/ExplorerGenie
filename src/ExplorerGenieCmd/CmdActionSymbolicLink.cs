// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action can add a directory as a symbolic link. Because the creation of new symbolic
    /// links requires elevation, the application will be restarted with admin privileges.
    /// </summary>
    internal class CmdActionSymbolicLink : CmdActionBase, ICmdAction
    {
        /// <inheritdoc/>
        public void Execute(List<string> filenames)
        {
            if (!TryGetDirectoryPath(filenames, out string clickedDirectory))
                return;

            FolderBrowserDialog folderDialog = new FolderBrowserDialog
            {
                Description = Language["guiNtfsSymbolicLinkPickFolder"],
                ShowNewFolderButton = false,
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string linkTargetDirectory = folderDialog.SelectedPath;
                if (IsBaseDirectoryOf(clickedDirectory, linkTargetDirectory))
                {
                    MessageBox.Show(Language["errNtfsSymbolicLinkRecursive"], Language["menuSymbolicLink"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string commandlineArguments = string.Format(
                    @"-NewSymbolicLinkElevated ""{0}"" ""{1}"" ""{2}""",
                    clickedDirectory, clickedDirectory, linkTargetDirectory);

                // Restart application elevated with admin privileges
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Arguments = commandlineArguments,
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception)
                {
                    // Probably not elevated by the user, the called action is responsible to inform the user.
                }
            }
        }

        private static bool TryGetDirectoryPath(List<string> filenames, out string directory)
        {
            directory = null;
            if (filenames.Count == 1)
            {
                directory = filenames[0];
                bool isDirectory = Directory.Exists(directory);
                return isDirectory;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the <paramref name="candidateDirectory"/> contains the <paramref name="baseDirectory"/>
        /// and therefore is a sibling of it. This method does not cover all possible quirks of the
        /// file system, but it takes care of some obvious mistakes.
        /// </summary>
        /// <param name="candidateDirectory">Directory to check.</param>
        /// <param name="baseDirectory">Check if this base is part of the candidate.</param>
        /// <returns>Returns true if it is the base path, otherwise false.</returns>
        private static bool IsBaseDirectoryOf(string candidateDirectory, string baseDirectory)
        {
            return candidateDirectory.StartsWith(baseDirectory, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
