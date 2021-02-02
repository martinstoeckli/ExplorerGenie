// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// ViewModel for the "System folders" tab.
    /// </summary>
    internal class PageSystemFoldersViewModel : ViewModelBaseWithLanguage
    {
        private List<SystemFolderViewModel> _systemFolders;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSystemFoldersViewModel"/> class.
        /// </summary>
        /// <param name="language">The shared language service from <see cref="SettingsViewModel"/>.</param>
        public PageSystemFoldersViewModel(ILanguageService language)
        {
            Language = language;
            GotoSystemFolderCommand = new RelayCommand<SystemFolderViewModel>(GotoSystemFolder);
        }

        /// <summary>
        /// Gets or cretes a list of system folders.
        /// </summary>
        public List<SystemFolderViewModel> SystemFolders
        {
            get
            {
                if (_systemFolders == null)
                {
                    _systemFolders = new List<SystemFolderViewModel>();

                    var foldersOfInterest = new Environment.SpecialFolder[]
                    {
                        Environment.SpecialFolder.ProgramFiles,
                        Environment.SpecialFolder.ProgramFilesX86,
                        Environment.SpecialFolder.CommonProgramFiles,
                        Environment.SpecialFolder.CommonProgramFilesX86,
                        Environment.SpecialFolder.ApplicationData,
                        Environment.SpecialFolder.LocalApplicationData,
                        Environment.SpecialFolder.CommonApplicationData,
                        Environment.SpecialFolder.MyDocuments,
                        Environment.SpecialFolder.MyMusic,
                        Environment.SpecialFolder.MyPictures,
                        Environment.SpecialFolder.MyVideos,
                        Environment.SpecialFolder.CommonDocuments,
                        Environment.SpecialFolder.CommonMusic,
                        Environment.SpecialFolder.CommonPictures,
                        Environment.SpecialFolder.CommonVideos,
                        Environment.SpecialFolder.Windows,
                        Environment.SpecialFolder.System,
                        Environment.SpecialFolder.SystemX86,
                        Environment.SpecialFolder.Desktop,
                        Environment.SpecialFolder.DesktopDirectory,
                        Environment.SpecialFolder.CommonDesktopDirectory,
                        Environment.SpecialFolder.StartMenu,
                        Environment.SpecialFolder.CommonStartMenu,
                        Environment.SpecialFolder.Startup,
                        Environment.SpecialFolder.CommonStartup,
                        Environment.SpecialFolder.Recent,
                        Environment.SpecialFolder.SendTo,
                        Environment.SpecialFolder.UserProfile,
                        Environment.SpecialFolder.Favorites,
                        Environment.SpecialFolder.InternetCache,
                        Environment.SpecialFolder.Cookies,
                        Environment.SpecialFolder.History,
                    };

                    // List folders of interest
                    foreach (Environment.SpecialFolder folder in foldersOfInterest)
                    {
                        string path = Environment.GetFolderPath(folder);
                        if (!string.IsNullOrEmpty(path))
                            _systemFolders.Add(new SystemFolderViewModel { Name = folder.ToString(), Path = path });
                    }

                    // Add temp path
                    _systemFolders.Insert(4, new SystemFolderViewModel { Name = "Temp", Path = Path.GetTempPath() });
                }
                return _systemFolders;
            }
        }

        /// <summary>
        /// Gets the command which opens the Explorer in a given special folder.
        /// </summary>
        public ICommand GotoSystemFolderCommand { get; private set; }

        private void GotoSystemFolder(SystemFolderViewModel systemFolder)
        {
            string arguments = string.Format("/root,\"{0}\"", systemFolder.Path);
            Process.Start("explorer.exe", arguments);
        }
    }
}
