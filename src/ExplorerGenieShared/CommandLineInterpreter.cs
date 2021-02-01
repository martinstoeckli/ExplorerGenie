// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Command line parser of the ExplorerGenieCmd.
    /// </summary>
    internal class CommandLineInterpreter
    {
        /// <summary>
        /// Interprets the command line and recreates the arguments.
        /// </summary>
        /// <param name="commandLine">The command line of the application, pass Environment.CommandLine.</param>
        /// <returns>The parsed command line arguments.</returns>
        public static CommandLineArgs ParseCommandLine(string commandLine)
        {
            CommandLineArgs result = new CommandLineArgs();

            List<string> parts = SplitCommandLine(commandLine);
            RemoveOwnExeName(parts);

            if (ArgumentIsAction(parts, 0))
            {
                result.Action = parts[0];
                parts.RemoveAt(0);
            }

            // The common directory must be the first parameter, followed by 1-n filenames.
            if (parts.Count >= 2)
            {
                string directory = parts[0];
                for (int index = 1; index < parts.Count; index++)
                {
                    string filePath = Path.Combine(directory, parts[index]);
                    result.Filenames.Add(filePath);
                }
            }
            return result;
        }

        private static void RemoveOwnExeName(List<string> parts)
        {
            string assemblyName = Assembly.GetExecutingAssembly().Location;
            if ((parts.Count >= 1) && parts[0].Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                parts.RemoveAt(0);
        }

        private static bool ArgumentIsAction(List<string> parts, int index)
        {
            return (parts.Count > index) && parts[index].StartsWith("-");
        }

        /// <summary>
        /// Interprets the command line created by the ExplorerGenieExt menu shell extension.
        /// </summary>
        /// <param name="commandLine">The command line. It can include an option parameter and
        /// a list of filepaths, each filename is enclosed in double quotes. Since file paths
        /// cannot include double quotes, escaping is not necessary.</param>
        /// <returns>List of parameters without double quotes.</returns>
        internal static List<string> SplitCommandLine(string commandLine)
        {
            const char quote = '"';
            const char separator = ' ';

            // Terminate command line for easier interpretation
            commandLine = commandLine.Trim();
            if (!string.IsNullOrEmpty(commandLine))
                commandLine += separator;

            List<string> result = new List<string>();
            StringBuilder currentParameter = new StringBuilder();

            bool insideQuote = false;
            for (int index = 0; index < commandLine.Length; index++)
            {
                char currentChar = commandLine[index];
                bool isQuote = currentChar == quote;
                bool isSeparator = currentChar == separator;

                if (isQuote)
                {
                    // Entering or leaving the quote scope
                    insideQuote = !insideQuote;
                }
                else if (isSeparator)
                {
                    if (insideQuote)
                    {
                        // Separator inside a quote is handled as normal character.
                        currentParameter.Append(currentChar);
                    }
                    else
                    {
                        // Separator which separates two parameters.
                        if (currentParameter.Length > 0)
                            result.Add(currentParameter.ToString());
                        currentParameter.Clear();
                    }
                }
                else
                {
                    // Normal character.
                    currentParameter.Append(currentChar);
                }
            }
            return result;
        }
    }
}
