// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
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
        }
    }
}
