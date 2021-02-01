// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExplorerGenieShared;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// This action opens a predefined or a user defined tool.
    /// </summary>
    internal class CmdActionOpenTool : ICmdAction
    {
        private readonly ISettingsService _settingsService;
        private readonly bool _isCustomTool;
        private readonly int _toolIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdActionOpenTool"/> class.
        /// </summary>
        /// <param name="settingsService">A service which can store the settings.</param>
        /// <param name="commandLineAction">The action parameter of the command line.</param>
        public CmdActionOpenTool(ISettingsService settingsService, string commandLineAction)
        {
            _settingsService = settingsService;
            ParseCommandLineAction(commandLineAction, out _isCustomTool, out _toolIndex);
        }

        /// <inheritdoc/>
        public void Execute(List<string> filenames)
        {
            new FilenameSorter().Sort(filenames);
            string filename = filenames.FirstOrDefault();
            if (string.IsNullOrEmpty(filename))
                return;

            bool isDirectory = Directory.Exists(filename);
            CustomGotoToolModel toolModel = _isCustomTool
                ? _settingsService.LoadSettingsOrDefault().CustomGotoTools[_toolIndex]
                : BuildPreDefinedTool(_toolIndex, isDirectory);

            ProcessStartInfo startInfo = GotoToolStarter.Prepare(toolModel, filename);
            GotoToolStarter.StartProcess(startInfo);
        }

        /// <summary>
        /// Creates a pseudo custom tool, with the parameters of a predefined tool.
        /// </summary>
        /// <param name="toolIndex">Index of the predefined tool.</param>
        /// <param name="isDirectory">Determines whether the selected context menu item is a
        /// directory or a file.</param>
        /// <returns>Predefined tool.</returns>
        private CustomGotoToolModel BuildPreDefinedTool(int toolIndex, bool isDirectory)
        {
            bool runAsAdmin = (toolIndex % 2) > 0; // Odd entries are admin commands
            switch (toolIndex)
            {
                case 0:
                case 1:
                    return new CustomGotoToolModel { CommandLine = @"cmd.exe /k ""cd /d {D}""", AsAdmin = runAsAdmin };
                case 2:
                case 3:
                    return new CustomGotoToolModel { CommandLine = @"powershell.exe -noexit -command Set-Location -LiteralPath '{D}'", AsAdmin = runAsAdmin };
                case 4:
                case 5:
                    if (isDirectory)
                        return new CustomGotoToolModel { CommandLine = @"explorer.exe /root,""{P}""", AsAdmin = runAsAdmin };
                    else
                        return new CustomGotoToolModel { CommandLine = @"explorer.exe /select,""{P}""", AsAdmin = runAsAdmin };
                default:
                    return null;
            }
        }

        /// <summary>
        /// The context menu will pass an action parameter containing the keyword "-OpenTool",
        /// followed by "-U" (user defined) or "-P" (pre defined), and a trailing number.
        /// <example>
        /// -OpenTool-U-2
        /// => user defined tool with index 2
        /// </example>
        /// </summary>
        /// <param name="commandLineAction">The action parameter of the command line.</param>
        /// <param name="isCustomTool">Receives a value indicating whether the requested tool is
        /// a user defined tool, or a predefined tool.</param>
        /// <param name="toolIndex">Receives the index of the requested tool.</param>
        private static void ParseCommandLineAction(string commandLineAction, out bool isCustomTool, out int toolIndex)
        {
            string[] parts = commandLineAction.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                throw new ArgumentException(nameof(commandLineAction));

            isCustomTool = "U".Equals(parts[1], StringComparison.OrdinalIgnoreCase);
            toolIndex = int.Parse(parts[2]);
        }
    }
}
