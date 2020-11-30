// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace ExplorerGenieShared.Models
{
    /// <summary>
    /// Enumeration of all known formats for the path for the copy file command.
    /// </summary>
    public enum CopyFileFormat
    {
        /// <summary>Copies the path in normal form</summary>
        OriginalPath = 0,

        /// <summary>Copies the path as URI</summary>
        Uri = 1,

        /// <summary>Copies the path in escaped C/C++ syntax.</summary>
        C = 2,

        /// <summary>Copies the path with slashes for Html files.</summary>
        Html = 3,
    }
}
