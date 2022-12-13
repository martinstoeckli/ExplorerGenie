// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// View model for the settings of ExplorerGenie.
    /// </summary>
    internal class SettingsViewModel : ViewModelBaseWithLanguage, IDisposable
    {
        private readonly SettingsModel _model;
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
            : this(new SettingsService(), CommandLineInterpreter.ParseCommandLine(Environment.CommandLine).Filenames)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        /// <param name="settingsService">A service which can store the settings.</param>
        /// <param name="filenames">User selected list of files, passed by the menu extension.</param>
        public SettingsViewModel(ISettingsService settingsService, List<string> filenames)
        {
            _settingsService = settingsService;
            new FilenameSorter().Sort(filenames);
            _model = _settingsService.LoadSettingsOrDefault();

            GotoToolPageViewModel = new PageGotoToolViewModel(_model, Language, _settingsService);
            CopyPathPageViewModel = new PageCopyPathViewModel(_model, Language, _settingsService, filenames);
            CalculateHashPageViewModel = new PageCalculateHashViewModel(_model, Language, _settingsService, filenames);
            NtfsPageViewModel = new PageNtfsViewModel(_model, Language, _settingsService, filenames);
            SystemFoldersPageViewModel = new PageSystemFoldersViewModel(Language);

            OpenHomepageCommand = new RelayCommand(OpenHomepage);
            CloseCommand = new RelayCommand<Window>(CloseWindow);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        ~SettingsViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            GotoToolPageViewModel.Dispose();
        }

        /// <inheritdoc cref="SettingsModel.FreshInstallation"/>
        public bool FreshInstallation
        {
            get { return _model.FreshInstallation; }

            set
            {
                if (SetPropertyIndirect(() => _model.FreshInstallation, (v) => _model.FreshInstallation = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }

        public PageGotoToolViewModel GotoToolPageViewModel { get; private set; }

        public PageCopyPathViewModel CopyPathPageViewModel { get; private set; }

        public PageCalculateHashViewModel CalculateHashPageViewModel { get; private set; }

        public PageNtfsViewModel NtfsPageViewModel { get; private set; }

        public PageSystemFoldersViewModel SystemFoldersPageViewModel { get; private set; }

        /// <summary>
        /// Gets the command to close the application.
        /// </summary>
        public ICommand CloseCommand { get; private set; }

        private void CloseWindow(Window callerWindow)
        {
            callerWindow.Close();
        }

        public string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
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
    }
}
