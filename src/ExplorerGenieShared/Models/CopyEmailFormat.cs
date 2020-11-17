// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace ExplorerGenieShared.Models
{
    /// <summary>
    /// Enumeration of all known formats for the path for the copy email command.
    /// </summary>
    public enum CopyEmailFormat
    {
        /// <summary>Copies the path for pasting in Outlook</summary>
        Outlook = 0,

        /// <summary>Copies the path for pasting in Thunderbird or other email clients.</summary>
        Thunderbird = 1,
    }
}
