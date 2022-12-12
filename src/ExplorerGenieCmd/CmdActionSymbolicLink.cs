// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action can add a directory as a symbolic link. Because the creation of new symbolic
    /// links requires elevation, the application will be restarted with admin privileges.
    /// </summary>
    internal class CmdActionSymbolicLink : ICmdAction
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdActionSymbolicLink"/> class.
        /// </summary>
        /// <param name="settingsService">A service which can store the settings.</param>
        /// <param name="commandLineAction">The action parameter of the command line.</param>
        public CmdActionSymbolicLink(ISettingsService settingsService, string commandLineAction)
        {
            _settingsService = settingsService;
        }

        /// <inheritdoc/>
        public void Execute(List<string> filenames)
        {
            if (!TryGetDirectoryPath(filenames, out string parentDirectory))
                return;

            FolderBrowserDialog folderDialog = new FolderBrowserDialog
            {
                Description = "Select the directory which should be added as symbolic link.",
                ShowNewFolderButton = false,
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string childDirectory = folderDialog.SelectedPath;
                string commandlineArguments = string.Format(
                    @"-NewSymbolicLinkElevated ""{0}"" ""{1}"" ""{2}""",
                    parentDirectory, parentDirectory, childDirectory);

                // Restart application elevated with admin privileges
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Arguments = commandlineArguments,
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process.Start(startInfo);
            }
        }

        private bool TryGetDirectoryPath(List<string> filenames, out string directory)
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
    }
}
