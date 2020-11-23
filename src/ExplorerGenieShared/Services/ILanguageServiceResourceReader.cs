// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace ExplorerGenieShared.Services
{
    /// <summary>
    /// Used by a <see cref="ILanguageService"/> to read the resources.
    /// </summary>
    public interface ILanguageServiceResourceReader
    {
        /// <summary>
        /// Implementations should return a dictionary with text resources of a given language.
        /// The dictionary should be case insensitive, but it is recommended to use lower case only.
        /// </summary>
        /// <param name="languageCode">Two digit lanugage code of the required language.</param>
        /// <returns>A dictionary containing the text resources, or null if no such resources could
        /// be found.</returns>
        Dictionary<string, string> LoadTextResources(string languageCode);
    }
}
