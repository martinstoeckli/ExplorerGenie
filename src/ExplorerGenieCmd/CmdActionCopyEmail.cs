// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using ExplorerGenieShared;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action copies the filename(s) to the Windows clipboard, to be used inside emails.
    /// </summary>
    internal class CmdActionCopyEmail : CmdActionBase, ICmdAction
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdActionCopyEmail"/> class.
        /// </summary>
        /// <param name="settingsService">A service which can store the settings.</param>
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

            PathUtils.ConvertForCopyEmailAction(filenames, settings);

            string clipboardText = string.Join(separator, filenames);
            Win32ApiClipboard.TrySetText(clipboardText);
        }
    }
}
