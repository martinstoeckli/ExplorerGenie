// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Allows to execute a command line silently on the command prompt (cmd.exe).
    /// All commands known to the command prompt are available.
    /// </summary>
    interface ICommandPromptRunner
    {
        /// <summary>
        /// Runs a command silently on the command prompt.
        /// </summary>
        /// <param name="command">Command as one would type it in the command prompt.</param>
        /// <param name="elevated">Asks the user for elevated privileges before running the command.</param>
        void Run(string command, bool elevated);
    }

    /// <summary>
    /// Implementation of the <see cref="ICommandPromptRunner"/> interface.
    /// </summary>
    internal class CommandPromptRunner : ICommandPromptRunner
    {
        /// <inheritdoc/>
        public void Run(string command, bool elevated)
        {
            // Let the command prompt carry out the command and then terminate.
            command = "/c " + command;

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = command,
                Verb = elevated ? "runas" : "open",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            Process.Start(startInfo);
        }
    }
}
