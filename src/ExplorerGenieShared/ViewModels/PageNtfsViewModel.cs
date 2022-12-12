// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    internal class PageNtfsViewModel : ViewModelBaseWithLanguage
    {
        private readonly SettingsModel _model;
        private readonly ISettingsService _settingsService;
        private readonly List<string> _filenames;
        private List<FilenameOnlyViewModel> _filenamesForHash;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageNtfsViewModel"/> class.
        /// </summary>
        /// <param name="model">The shared model from <see cref="SettingsViewModel"/>.</param>
        /// <param name="language">The shared language service from <see cref="SettingsViewModel"/>.</param>
        /// <param name="settingsService">The shared settings service from <see cref="SettingsViewModel"/>.</param>
        /// <param name="filenames">User selected list of files, passed by the menu extension.</param>
        public PageNtfsViewModel(
            SettingsModel model,
            ILanguageService language,
            ISettingsService settingsService,
            List<string> filenames)
        {
            _model = model;
            Language = language;
            _settingsService = settingsService;
            _filenames = filenames;
        }

        /// <inheritdoc cref="SettingsModel.NewFolderShowMenu"/>
        public bool NewFolderShowMenu
        {
            get { return _model.NewFolderShowMenu; }

            set
            {
                if (SetPropertyIndirect(() => _model.NewFolderShowMenu, (v) => _model.NewFolderShowMenu = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }

        /// <inheritdoc cref="SettingsModel.HashShowMenu"/>
        public bool SymbolicLinkShowMenu
        {
            get { return _model.SymbolicLinkShowMenu; }

            set
            {
                if (SetPropertyIndirect(() => _model.SymbolicLinkShowMenu, (v) => _model.SymbolicLinkShowMenu = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }
    }
}
