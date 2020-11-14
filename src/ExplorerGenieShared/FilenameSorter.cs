// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Sorts a list of filenames 
    /// </summary>
    public class FilenameSorter
    {
        private Func<string, bool> _isDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilenameSorter"/> class.
        /// </summary>
        public FilenameSorter()
            : this(Directory.Exists)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilenameSorter"/> class.
        /// </summary>
        /// <param name="isDirectory">Delegate which can decide whether the path is a directory
        /// or a file. This makes the class independend of the file system and testable.</param>
        public FilenameSorter(Func<string, bool> isDirectory)
        {
            _isDirectory = isDirectory;
        }

        /// <summary>
        /// In-place sorting of a list of file/directory paths. Directories will be sorted to the
        /// top, files at the bottom, the comparison is done case insensitive.
        /// </summary>
        /// <param name="filenames">List of file/directory paths to sort.</param>
        public void Sort(List<string> filenames)
        {
            if ((filenames == null) || (filenames.Count < 2))
                return;

            // Sort directories and files separately, and keep checks for directory to a minimum.
            List<string> directories = new List<string>();
            List<string> files = new List<string>();
            foreach (string filename in filenames)
            {
                if (_isDirectory(filename))
                    directories.Add(filename);
                else
                    files.Add(filename);
            }

            directories.Sort(StringComparer.InvariantCultureIgnoreCase);
            files.Sort(StringComparer.InvariantCultureIgnoreCase);

            filenames.Clear();
            filenames.AddRange(directories);
            filenames.AddRange(files);
        }
    }
}
