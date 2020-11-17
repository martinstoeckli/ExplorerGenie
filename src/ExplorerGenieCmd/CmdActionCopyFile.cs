// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ExplorerGenieShared;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action copies the filename(s) to the Windows clipboard.
    /// </summary>
    internal class CmdActionCopyFile : ICmdAction
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdActionCopyFile"/> class.
        /// </summary>
        /// <param name="settingsService"></param>
        public CmdActionCopyFile(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <inheritdoc/>
        public void Execute(List<string> filenames)
        {
            const string separator = "\r\n";
            new FilenameSorter().Sort(filenames);
            SettingsModel settings = _settingsService.LoadSettingsOrDefault();

            // Convert to UNC path if requested
            if (settings.CopyFileConvertToUnc)
            {
                filenames.ForEach(file => PathUtils.ExpandUncFilename(file));
            }

            // Convert to choosen format
            switch (settings.CopyFileFormat)
            {
                case CopyFileFormat.OriginalPath:
                    break; // Already in the requested form
                case CopyFileFormat.Uri:
                    filenames.ForEach(file => PathUtils.ConvertToUri(file));
                    break;
                case CopyFileFormat.C:
                    filenames.ForEach(file => PathUtils.ConvertToC(file));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.CopyFileFormat));
            }

            // Convert to filenames only if requested
            if (settings.CopyFileOnlyFilename)
            {
                filenames.ForEach(file => Path.GetFileName(file));
            }

            string clipboardText = string.Join(separator, filenames);
            Clipboard.SetText(clipboardText);
        }
    }
}
