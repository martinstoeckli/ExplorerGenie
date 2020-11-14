// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// Interface for all command actions.
    /// </summary>
    internal interface ICmdAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="filenames">List of filenames passed from the ExplorerGenieExt menu
        /// extension.</param>
        void Execute(List<string> filenames);
    }
}
