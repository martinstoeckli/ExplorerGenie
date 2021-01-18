// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExplorerGenieShared.Services
{
    /// <summary>
    /// Implementation of the <see cref="ILanguageServiceResourceReader"/> interface, which can load
    /// text resoures from language files. It searches for resource files of the format:
    /// <example>
    /// {ResourceDirectoryPath}\Lng.{Domain}.{LanguageCode}
    /// D:\AppDirectory\Lng.AppName.en
    /// </example>
    /// If no file for the requested language can be found, the reader checks whether there exists a
    /// fallback resource file in the embedded resources. So you can e.g. copy the english language
    /// file to the output directory, as well as compiling it as "embedded resource", just set the
    /// build action for this file to "Embedded Resource" instead of "AdditionalFiles".
    /// </summary>
    public class LanguageServiceFileResourceReader : ILanguageServiceResourceReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageServiceFileResourceReader"/> class.
        /// </summary>
        public LanguageServiceFileResourceReader()
        {
            string callingAssemblyFilePath = Assembly.GetCallingAssembly().Location;
            ResourceDirectoryPath = Path.GetDirectoryName(callingAssemblyFilePath);
            Domain = Path.GetFileNameWithoutExtension(callingAssemblyFilePath);
        }

        /// <summary>
        /// Gets or sets the directory containing the language files. This is preset to the path
        /// of the calling assembly.
        /// </summary>
        public string ResourceDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the domain of the language resource. For this reader the domain is preset
        /// to the name of the calling assembly without the file extension.
        /// </summary>
        public string Domain { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, string> LoadTextResources(string languageCode)
        {
            Dictionary<string, string> result = null;

            string resourceFilePath = BuildResourceFilePath(ResourceDirectoryPath, Domain, languageCode);
            if (File.Exists(resourceFilePath))
            {
                // Read from the found language file.
                using (StreamReader resourceStream = new StreamReader(resourceFilePath, Encoding.UTF8))
                {
                    result = ReadFromStream(resourceStream);
                }
            }
            else
            {
                // Check if assembly has an embedded resource which can be used as fallback.
                if (TryFindEmbeddedLanguageResourceName(resourceFilePath, out string resourceName))
                {
                    // Read from the embedded language file.
                    using (Stream resourceStream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName))
                    using (StreamReader reader = new StreamReader(resourceStream, Encoding.UTF8))
                    {
                        result = ReadFromStream(reader);
                    }
                }
            }
            return result;
        }

        internal Dictionary<string, string> ReadFromStream(StreamReader languageResourceStream)
        {
            var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            // process all lines
            string line;
            while ((line = languageResourceStream.ReadLine()) != null)
            {
                if (!IsComment(line) &&
                    (TrySplitLine(line, out string resKey, out string resText)))
                {
                    resText = ReplaceSpecialTags(resText);
                    result[resKey] = resText;
                }
            }
            return result;
        }

        /// <summary>
        /// Checks whether a given line contains a comment starting with //, and therefore can be
        /// skipped.
        /// </summary>
        /// <param name="line">Line to test.</param>
        /// <returns>Returns true if the line contains a comment, otherwise false.</returns>
        private bool IsComment(string line)
        {
            return line.TrimStart().StartsWith(@"//");
        }

        /// <summary>
        /// Splits a line from a file in two parts, id and text. The id is expected at the begin of
        /// the line, separated by a space. It's allowed to have none or multiple whitespace
        /// characters instead of the recommended single space.
        /// </summary>
        /// <param name="line">Line form resource file.</param>
        /// <param name="key">Retreives the found id.</param>
        /// <param name="text">Retreives the found text.</param>
        /// <returns>Returns true if line was valid, otherwise false.</returns>
        internal static bool TrySplitLine(string line, out string key, out string text)
        {
            key = null;
            text = null;
            if (string.IsNullOrWhiteSpace(line))
                return false;

            line = line.Trim();
            int delimiterPos = line.IndexOf(' ');
            if (delimiterPos < 1)
                return false;

            key = line.Substring(0, delimiterPos).TrimEnd();
            text = line.Substring(delimiterPos + 1).TrimStart();
            return true;
        }

        /// <summary>
        /// Replaces tags with a special meaning (e.g. "\n" or "\r\n" becomes crlf).
        /// </summary>
        /// <param name="resText">Text to search.</param>
        /// <returns>Text with replaces tags.</returns>
        protected string ReplaceSpecialTags(string resText)
        {
            string result = resText;
            if (result.Contains(@"\n"))
            {
                result = result.Replace(@"\r\n", "\r\n");
                result = result.Replace(@"\n", "\r\n");
            }
            return result;
        }

        /// <summary>
        /// Builds a filename for specific resourcefile, but does not check whether that file
        /// really exists.
        /// </summary>
        /// <param name="directory">Search directory of resource file.</param>
        /// <param name="domain">Domain to which the resource belongs to.</param>
        /// <param name="languageCode">Two letter language code of the resource.</param>
        /// <returns>Expected file path of the resource file.</returns>
        private string BuildResourceFilePath(string directory, string domain, string languageCode)
        {
            string resFileName = string.Format("Lng.{0}.{1}", domain, languageCode);
            return Path.Combine(directory, resFileName);
        }

        /// <summary>
        /// Tries to find an embedded resource file with the same name as the requested language
        /// resource file. If no exact file can be found it looks for files with text resources of
        /// any another lanugage (fallback language).
        /// </summary>
        /// <param name="resourceFilePath">Path of requested language file.</param>
        /// <param name="resourceName">If an embedded resource cold be found, its name is returned.</param>
        /// <returns>Resturns true if a matching resource file could be found, otherwise false.</returns>
        private bool TryFindEmbeddedLanguageResourceName(string resourceFilePath, out string resourceName)
        {
            string[] resourceNames = Assembly.GetCallingAssembly().GetManifestResourceNames();

            // Look for an exact match
            string resourceNamePattern = Path.GetFileName(resourceFilePath);
            resourceName = resourceNames.FirstOrDefault(item => item.EndsWith(resourceNamePattern));

            // Look for a match with another language (maybe just the default language is embedded).
            if (resourceName == null)
            {
                resourceNamePattern = Path.GetFileNameWithoutExtension(resourceFilePath);
                resourceName = resourceNames.FirstOrDefault(item => item.Contains(resourceNamePattern));
            }
            return !string.IsNullOrEmpty(resourceName);
        }
    }
}
