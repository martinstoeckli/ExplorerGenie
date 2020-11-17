// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Windows;
using ExplorerGenieShared;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    internal class CmdActionCopyEmail : ICmdAction
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdActionCopyEmail"/> class.
        /// </summary>
        /// <param name="settingsService"></param>
        public CmdActionCopyEmail(ISettingsService settingsService)
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
            if (settings.CopyEmailConvertToUnc)
            {
                filenames.ForEach(file => PathUtils.ExpandUncFilename(file));
            }

            // Convert to choosen format
            switch (settings.CopyEmailFormat)
            {
                case CopyEmailFormat.Outlook:
                    filenames.ForEach(file => PathUtils.ConvertToOutlook(file));
                    break;
                case CopyEmailFormat.Thunderbird:
                    filenames.ForEach(file => PathUtils.ConvertToUri(file));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.CopyEmailFormat));
            }

            string clipboardText = string.Join(separator, filenames);
            Clipboard.SetText(clipboardText);
        }
    }
}
