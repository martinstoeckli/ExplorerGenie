// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ExplorerGenieShared;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action can add a directory as a symbolic link. Because the creation of new symbolic
    /// links requires elevation, the application will be restarted with admin privileges.
    /// </summary>
    internal class CmdActionSymbolicLink : CmdActionBase, ICmdAction
    {
        private ICommandPromptRunner _commandPrompt;

        /// <summary>
        /// Initialize a new instance of the <see cref="CmdActionSymbolicLink"/> class.
        /// </summary>
        /// <param name="commandPrompt"></param>
        public CmdActionSymbolicLink(ICommandPromptRunner commandPrompt)
        {
            _commandPrompt = commandPrompt;
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <remarks>
        /// Calling the Windows function "CreateSymbolicLink()" would be the easiest solution, but
        /// unfortunately it runs only with elevated admin provileges. Since apps in the store are
        /// not allowed to ask for elevation (allowElevation capability), the only way to create a
        /// symbolic link is to open an elevated command prompt and call the "mklink" command.
        /// </remarks>
        /// <param name="filenames">List of filenames passed from the ExplorerGenieExt menu
        /// extension.</param>
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

                string symbolicLinkDirectory = Path.Combine(clickedDirectory, Path.GetFileName(linkTargetDirectory));
                if (Directory.Exists(symbolicLinkDirectory))
                {
                    MessageBox.Show(Language.LoadTextFmt("guiNtfsDirectoryExists", symbolicLinkDirectory), Language["menuSymbolicLink"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string commandlineArguments = string.Format(
                    @"mklink /D ""{0}"" ""{1}""",
                    symbolicLinkDirectory,
                    linkTargetDirectory);

                try
                {
                    _commandPrompt.Run(commandlineArguments, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Language["menuSymbolicLink"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
