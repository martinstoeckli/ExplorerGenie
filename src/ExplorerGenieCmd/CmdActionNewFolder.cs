// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.IO;
using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action can add a new folder to the selected directory.
    /// </summary>
    internal class CmdActionNewFolder : ICmdAction
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdActionNewFolder"/> class.
        /// </summary>
        /// <param name="settingsService">A service which can store the settings.</param>
        public CmdActionNewFolder(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <inheritdoc/>
        public void Execute(List<string> filenames)
        {
            if (!TryGetDirectoryPath(filenames, out string directory))
                return;
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
