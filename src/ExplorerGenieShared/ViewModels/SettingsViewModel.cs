// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// View model for the settings of ExplorerGenie.
    /// </summary>
    internal class SettingsViewModel : ViewModelBaseWithLanguage
    {
        private readonly SettingsModel _model;
        private readonly ISettingsService _settingsService;
        private string _copyFileExample;
        private string _copyEmailExample;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
            : this(new SettingsModel(), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        /// <param name="model">The related model.</param>
        /// <param name="settingsService">A service which can store the settings.</param>
        public SettingsViewModel(SettingsModel model, ISettingsService settingsService)
        {
            _model = model;
            _settingsService = settingsService;
            OpenHomepageCommand = new RelayCommand(OpenHomepage);
            UpdateCopyFileExample();
            UpdateCopyEmailExample();
        }

        /// <summary>
        /// Gets the command which opens the homepage.
        /// </summary>
        public ICommand OpenHomepageCommand { get; private set; }

        private void OpenHomepage(object obj)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "https://www.martinstoeckli.ch/explorergenie",
                UseShellExecute = true,
            };
            Process.Start(startInfo);
        }

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
            string documentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentDirectory, "My Document.txt");
        }

        public bool OpenInShowMenu
        {
            get { return _model.OpenInShowMenu; }

            set
            {
                if (SetPropertyIndirect(() => _model.OpenInShowMenu, (v) => _model.OpenInShowMenu = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }
    }
}
