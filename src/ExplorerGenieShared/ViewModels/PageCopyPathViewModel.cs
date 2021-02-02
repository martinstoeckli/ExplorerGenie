// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// ViewModel for the "Copy path" tab.
    /// </summary>
    internal class PageCopyPathViewModel : ViewModelBaseWithLanguage
    {
        private readonly SettingsModel _model;
        private readonly ISettingsService _settingsService;
        private readonly List<string> _filenames;
        private string _copyFileExample;
        private string _copyEmailExample;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageCopyPathViewModel"/> class.
        /// </summary>
        /// <param name="model">The shared model from <see cref="SettingsViewModel"/>.</param>
        /// <param name="language">The shared language service from <see cref="SettingsViewModel"/>.</param>
        /// <param name="settingsService">The shared settings service from <see cref="SettingsViewModel"/>.</param>
        /// <param name="filenames">User selected list of files, passed by the menu extension.</param>
        public PageCopyPathViewModel(
            SettingsModel model,
            ILanguageService language,
            ISettingsService settingsService,
            List<string> filenames)
        {
            _model = model;
            Language = language;
            _settingsService = settingsService;
            _filenames = filenames;

            CopyTextToClipboardCommand = new RelayCommand<string>(CopyTextToClipboard);

            UpdateCopyFileExample();
            UpdateCopyEmailExample();
        }

        /// <summary>
        /// Gets the command which copies a text to the clipboard.
        /// </summary>
        public ICommand CopyTextToClipboardCommand { get; private set; }

        private void CopyTextToClipboard(string text)
        {
            Clipboard.SetText(text);
        }

        /// <inheritdoc cref="SettingsModel.CopyFileShowMenu"/>
        public bool CopyFileShowMenu
        {
            get { return _model.CopyFileShowMenu; }

            set
            {
                if (SetPropertyIndirect(() => _model.CopyFileShowMenu, (v) => _model.CopyFileShowMenu = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }

        /// <inheritdoc cref="SettingsModel.CopyFileFormat"/>
        public CopyFileFormat CopyFileFormat
        {
            get { return _model.CopyFileFormat; }

            set
            {
                if (SetPropertyIndirect(() => _model.CopyFileFormat, (v) => _model.CopyFileFormat = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                    UpdateCopyFileExample();
                }
            }
        }

        /// <inheritdoc cref="SettingsModel.CopyFileOnlyFilename"/>
        public bool CopyFileOnlyFilename
        {
            get { return _model.CopyFileOnlyFilename; }

            set
            {
                if (SetPropertyIndirect(() => _model.CopyFileOnlyFilename, (v) => _model.CopyFileOnlyFilename = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                    UpdateCopyFileExample();
                }
            }
        }

        /// <inheritdoc cref="SettingsModel.CopyFileConvertToUnc"/>
        public bool CopyFileConvertToUnc
        {
            get { return _model.CopyFileConvertToUnc; }

            set
            {
                if (SetPropertyIndirect(() => _model.CopyFileConvertToUnc, (v) => _model.CopyFileConvertToUnc = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                    UpdateCopyFileExample();
                }
            }
        }

        /// <summary>
        /// Gets or sets the preview of the "copy file" action.
        /// </summary>
        public string CopyFileExample
        {
            get { return _copyFileExample; }

            set { SetProperty(ref _copyFileExample, value, false); }
        }

        private void UpdateCopyFileExample()
        {
            List<string> examplePaths = new List<string>() { GenerateExamplePath() };
            PathUtils.ConvertForCopyFileAction(examplePaths, _model);
            CopyFileExample = examplePaths[0];
        }

        /// <inheritdoc cref="SettingsModel.CopyEmailFormat"/>
        public CopyEmailFormat CopyEmailFormat
        {
            get { return _model.CopyEmailFormat; }

            set
            {
                if (SetPropertyIndirect(() => _model.CopyEmailFormat, (v) => _model.CopyEmailFormat = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                    UpdateCopyEmailExample();
                }
            }
        }

        /// <inheritdoc cref="SettingsModel.CopyEmailConvertToUnc"/>
        public bool CopyEmailConvertToUnc
        {
            get { return _model.CopyEmailConvertToUnc; }

            set
            {
                if (SetPropertyIndirect(() => _model.CopyEmailConvertToUnc, (v) => _model.CopyEmailConvertToUnc = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                    UpdateCopyEmailExample();
                }
            }
        }

        /// <summary>
        /// Gets or sets the preview of the "copy email" action.
        /// </summary>
        public string CopyEmailExample
        {
            get { return _copyEmailExample; }

            set { SetProperty(ref _copyEmailExample, value, false); }
        }

        private void UpdateCopyEmailExample()
        {
            List<string> examplePaths = new List<string>() { GenerateExamplePath() };
            PathUtils.ConvertForCopyEmailAction(examplePaths, _model);
            CopyEmailExample = examplePaths[0];
        }

        private string GenerateExamplePath()
        {
            if (_filenames.Count > 0)
            {
                return _filenames[0];
            }
            else
            {
                string documentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Combine(documentDirectory, "My Document.txt");
            }
        }
    }
}
