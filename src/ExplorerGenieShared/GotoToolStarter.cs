// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ExplorerGenieShared.Models;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Helper class for the GotoTool action.
    /// </summary>
    public static class GotoToolStarter
    {
        private static char[] _quotes = { '"', '\'' };

        /// <summary>
        /// Prepares the necessary information to start a new process using a given
        /// <see cref="CustomGotoToolModel"/>.
        /// </summary>
        /// <param name="toolModel">The goto tool model.</param>
        /// <param name="filename">The first filename passed from the menu shell extension.</param>
        /// <returns>A new startup information object.</returns>
        public static ProcessStartInfo Prepare(CustomGotoToolModel toolModel, string filename)
        {
            filename = filename.Trim();
            bool isDirectory = Directory.Exists(filename);
            if (isDirectory)
                filename = PathUtils.IncludeTrailingBackslash(filename);

            string fullPath = filename;
            string directoryOnly = isDirectory
                ? filename
                : PathUtils.IncludeTrailingBackslash(Path.GetDirectoryName(filename));
            string fileOnly = Path.GetFileName(filename);

            // Replace variables
            StringBuilder commandLineBuilder = new StringBuilder(toolModel.CommandLine);
            commandLineBuilder.Replace("{P}", fullPath);
            commandLineBuilder.Replace("{p}", fullPath);
            commandLineBuilder.Replace("{D}", directoryOnly);
            commandLineBuilder.Replace("{d}", directoryOnly);
            commandLineBuilder.Replace("{F}", fileOnly);
            commandLineBuilder.Replace("{f}", fileOnly);

            // Split command line into executable file and arguments
            string commandLine = commandLineBuilder.ToString();
            SplitCommandLine(commandLine, out string executableFile, out string arguments);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = executableFile,
                Arguments = arguments,
                UseShellExecute = true,
                Verb = toolModel.AsAdmin ? "runas" : "open",
                WindowStyle = ProcessWindowStyle.Normal,
            };
            return startInfo;
        }

        private static void SplitCommandLine(string commandLine, out string executableFile, out string arguments)
        {
            commandLine = commandLine.Trim();
            int executableLength = DetermineExecutableLength(commandLine);
            executableFile = commandLine.Substring(0, executableLength).Trim(_quotes);
            arguments = commandLine.Substring(executableLength).TrimStart();
        }

        /// <summary>
        /// Determines the split position between executable filename and arguments.
        /// The <paramref name="commandLine"/> must be trimmed before calling this function.
        /// </summary>
        /// <param name="commandLine">The command line to split.</param>
        /// <returns>The length of the executable filename, inclusive quotes if available.</returns>
        internal static int DetermineExecutableLength(string commandLine)
        {
            if (string.IsNullOrEmpty(commandLine))
                return 0;

            int result = 0;
            char? quote = null;
            foreach (char commandLineChar in commandLine)
            {
                bool isWhiteSpace = char.IsWhiteSpace(commandLineChar);
                bool isQuote = Array.IndexOf(_quotes, commandLineChar) >= 0;

                if (isQuote)
                {
                    bool isInsideQuote = quote.HasValue;
                    if (isInsideQuote)
                    {
                        if (quote.Value == commandLineChar)
                        {
                            result++;
                            return result; // Quote ends the executable
                        }
                        else
                        {
                            result++; // Unmatching quote belongs to executable
                        }
                    }
                    else
                    {
                        result++;
                        quote = commandLineChar; // Starting the quote
                    }
                }
                else if (isWhiteSpace)
                {
                    bool isInsideQuote = quote.HasValue;
                    if (isInsideQuote)
                        result++; // White space belongs to executable
                    else
                        return result; // White space ends the executable
                }
                else
                {
                    result++; // Normal character belongs to executable
                }
            }
            return result;
        }

        /// <summary>
        /// Start the goto tool.
        /// </summary>
        /// <param name="startInfo">Prepared process startup information.</param>
        public static void StartProcess(ProcessStartInfo startInfo)
        {
            if (string.IsNullOrEmpty(startInfo?.FileName))
                return;
            Process.Start(startInfo);
        }
    }
}
