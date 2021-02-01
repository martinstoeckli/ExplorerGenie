// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace ExplorerGenieShared
{
    /// <summary>
    /// The parameters passed in the command line from ExplorerGenieExt menu shell extension.
    /// </summary>
    internal class CommandLineArgs
    {
        private List<string> _filenames;

        /// <summary>
        /// Gets or sets the action, which always starts with an "-".
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets a a list of filenames.
        /// </summary>
        public List<string> Filenames
        {
            get { return _filenames ?? (_filenames = new List<string>()); }

            set { _filenames = value; }
        }
    }
}
