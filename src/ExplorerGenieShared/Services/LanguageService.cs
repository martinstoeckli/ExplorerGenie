// Copyright © 2018 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace ExplorerGenieShared.Services
{
    /// <summary>
    /// Base class for implementations of the <see cref="ILanguageService"/> interface.
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageServiceResourceReader _resourceReader;
        private readonly string _languageCode;
        private Dictionary<string, string> _textResources;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageService"/> class.
        /// </summary>
        /// <param name="resourceReader">A reader which knows about how to load language resources.</param>
        public LanguageService(ILanguageServiceResourceReader resourceReader)
            : this(resourceReader, GetWindowsLanguageCode())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageService"/> class.
        /// </summary>
        /// <param name="resourceReader">A reader which knows about how to load language resources.</param>
        /// <param name="languageCode">Two digit language code.</param>
        public LanguageService(ILanguageServiceResourceReader resourceReader, string languageCode)
        {
            _resourceReader = resourceReader;
            _languageCode = languageCode;
        }

        /// <inheritdoc/>
        public string this[string id]
        {
            get { return LoadText(id); }
        }

        /// <inheritdoc/>
        public string LoadText(string id)
        {
            if (LazyLoadTextResources() && _textResources.TryGetValue(id, out string text))
            {
                return text;
            }
            else
            {
                if (Debugger.IsAttached)
                    throw new Exception(string.Format("Could not find text resource {0}", id));
                else
                    return "Translated text not found";
            }
        }

        /// <inheritdoc/>
        public string LoadTextFmt(string id, params object[] args)
        {
            string text = LoadText(id);
            try
            {
                return string.Format(text, args);
            }
            catch
            {
                return text;
            }
        }

        private bool LazyLoadTextResources()
        {
            if (_textResources == null)
                _textResources = _resourceReader.LoadTextResources(_languageCode);
            return _textResources != null;
        }

        /// <summary>
        /// Returns the two letter language code (ISO 639-1), used by the current process.
        /// </summary>
        /// <returns>Windows language code, two letters, lowercase.</returns>
        protected static string GetWindowsLanguageCode()
        {
            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
        }
    }
}
