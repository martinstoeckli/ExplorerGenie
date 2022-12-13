// Copyright © 2020 Martin Stoeckli.
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
        /// <param name="commandLineAction">The action parameter passed from ExplorerGenieExt menu
        /// extension, this sould be a parameter with a leading "-" like "-CopyFile".</param>
        /// <returns>A new instance of the found action, or null if no such action could be found.</returns>
        public ICmdAction CreateAction(string commandLineAction)
        {
            if ("-CopyFile".Equals(commandLineAction, StringComparison.OrdinalIgnoreCase))
            {
                return new CmdActionCopyFile(_settingsService);
            }
            else if ("-CopyEmail".Equals(commandLineAction, StringComparison.OrdinalIgnoreCase))
            {
                return new CmdActionCopyEmail(_settingsService);
            }
            else if (commandLineAction.StartsWith("-GotoTool", StringComparison.OrdinalIgnoreCase))
            {
                return new CmdActionOpenTool(_settingsService, commandLineAction);
            }
            else if (commandLineAction.StartsWith("-NewFolder", StringComparison.OrdinalIgnoreCase))
            {
                return new CmdActionNewFolder();
            }
            else if (commandLineAction.StartsWith("-NewSymbolicLinkElevated", StringComparison.OrdinalIgnoreCase))
            {
                return new CmdActionSymbolicLinkElevated();
            }
            else if (commandLineAction.StartsWith("-NewSymbolicLink", StringComparison.OrdinalIgnoreCase))
            {
                return new CmdActionSymbolicLink();
            }
            else
            {
                return null;
            }
        }
    }
}
