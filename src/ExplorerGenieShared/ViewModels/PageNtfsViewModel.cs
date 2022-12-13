// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    internal class PageNtfsViewModel : ViewModelBaseWithLanguage
    {
        private readonly SettingsModel _model;
        private readonly ISettingsService _settingsService;
        private readonly List<string> _filenames;

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
            AlternativeDataStreams = CreateAdsInfos(_filenames);
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

        private List<AdsViewModel> CreateAdsInfos(List<string> filenames)
        {
            var result = new List<AdsViewModel>();
            if (filenames.Count > 0)
            {
                bool isDirectory = Directory.Exists(filenames[0]);
                string rootDirectory = isDirectory ? filenames[0] : Path.GetDirectoryName(filenames[0]);
                List<string> paths = Directory.EnumerateFileSystemEntries(rootDirectory).ToList();
                new FilenameSorter().Sort(paths);
                AddAdsInfos(result, paths);
            }
            return result;
        }

        private static void AddAdsInfos(List<AdsViewModel> result, IEnumerable<string> paths)
        {
            foreach (string path in paths)
                AddAdsInfo(result, path);
        }

        private static void AddAdsInfo(List<AdsViewModel> result, string path)
        {
            AdsViewModel item = new AdsViewModel
            {
                Directory = Path.GetDirectoryName(path),
                FileName = Path.GetFileName(path),
            };
            result.Add(item);
        }

        /// <summary>
        /// Gets a list of ads infos.
        /// </summary>
        public List<AdsViewModel> AlternativeDataStreams { get; }
    }
}
