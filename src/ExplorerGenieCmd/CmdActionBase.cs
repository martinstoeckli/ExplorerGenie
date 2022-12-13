// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using ExplorerGenieShared.Services;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// Base class for command actions.
    /// </summary>
    internal class CmdActionBase
    {
        private ILanguageService _language;

        /// <summary>
        /// Gets or creates a bindable indexed property to load localized text resources.
        /// </summary>
        public ILanguageService Language
        {
            get
            {
                if (_language == null)
                {
                    var languageResourceReader = new LanguageServiceFileResourceReader { Domain = "ExplorerGenie" };
                    _language = new LanguageService(languageResourceReader);
                }
                return _language;
            }
        }
    }
}
