﻿// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// Factory for creating the command actions.
    /// </summary>
    internal class CmdActionFactory
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdActionFactory"/> class.
        /// </summary>
        /// <param name="settingsService">A service which can store the settings.</param>
        public CmdActionFactory(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Creates a command action given by the option parameter in the command line.
        /// </summary>
        /// <param name="commandLineOption">The option parameter passed from ExplorerGenieExt menu
        /// extension, this sould be a parameter with a leading "-" like "-CopyFile".</param>
        /// <returns>A new instance of the found action, or null if no such action could be found.</returns>
        public ICmdAction CreateAction(string commandLineOption)
        {
            if ("-CopyFile".Equals(commandLineOption, StringComparison.OrdinalIgnoreCase))
            {
                return new CmdActionCopyFile(_settingsService);
            }
            else if ("-CopyEmail".Equals(commandLineOption, StringComparison.OrdinalIgnoreCase))
            {
                return new CmdActionCopyEmail(_settingsService);
            }
            else
            {
                return null;
            }
        }
    }
}
