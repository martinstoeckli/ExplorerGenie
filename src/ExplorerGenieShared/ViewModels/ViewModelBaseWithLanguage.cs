// Copyright © 2018 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// Extends the base view model with a language service.
    /// </summary>
    public class ViewModelBaseWithLanguage : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBaseWithLanguage"/> class.
        /// </summary>
        public ViewModelBaseWithLanguage()
        {
            var languageResourceReader = new LanguageServiceFileResourceReader();
            languageResourceReader.Domain = "ExplorerGenie";
            Language = new LanguageService(languageResourceReader);
        }

        /// <summary>
        /// Gets a bindable indexed property to load localized text resources.
        /// In Xaml one can use it like this:
        /// <code>
        /// Text="{Binding Language[TextResourceName]}"
        /// </code>
        /// In a Razor view (.cshtml) one can use it like this:
        /// <code>
        /// @Model.Language["TextResourceName"]
        /// </code>
        /// </summary>
        public ILanguageService Language { get; private set; }
    }
}
