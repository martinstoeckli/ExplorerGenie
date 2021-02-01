// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using ExplorerGenieShared;
using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This assembly is called by the ExplorerGenieExt menu extension. This separation is done
    /// because the menu shell extension runs in the explorer.exe process, to minimize the impact
    /// of the actions to the explorer process (performance and stability).
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            CommandLineArgs args = CommandLineInterpreter.ParseCommandLine(Environment.CommandLine);
            if (string.IsNullOrEmpty(args.Action) || (args.Filenames.Count == 0))
                return;

            // Make sure there are no short file formats in the list.
            args.Filenames.ModifyEach(file => Path.GetFullPath(file));

            // Create and execute action.
            CmdActionFactory factory = new CmdActionFactory(new SettingsService());
            ICmdAction cmdAction = factory.CreateAction(args.Action);
            cmdAction?.Execute(args.Filenames);
        }
    }
}
