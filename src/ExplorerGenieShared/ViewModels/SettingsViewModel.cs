// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    internal class SettingsViewModel : ViewModelBaseWithLanguage
    {
        private readonly SettingsModel _model;
        private readonly ISettingsService _settingsService;
        private List<string> _filenames;
        private string _copyFileExample;
        private string _copyEmailExample;
        private List<SystemFolderViewModel> _systemFolders;
        private List<FilepathViewModel> _filenamesForHash;
        private FilepathViewModel _selectedFilenameForHash;
        private string _hashCandidate;
        private bool? _hashVerified;

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
            _filenames = filenames;
            new FilenameSorter().Sort(_filenames);
            _model = _settingsService.LoadSettingsOrDefault();
            OpenHomepageCommand = new RelayCommand(OpenHomepage);
            CloseCommand = new RelayCommand<Window>(CloseWindow);
            GotoSystemFolderCommand = new RelayCommand<SystemFolderViewModel>(GotoSystemFolder);
            CopyHashToClipboardCommand = new RelayCommand<HashResultViewModel>(CopyHashToClipboard);
            PasteHashFromClipboardCommand = new RelayCommand(PasteHashFromClipboard);
            HashResults = new ObservableCollection<HashResultViewModel>();

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

        /// <summary>
        /// Gets the command to close the application.
        /// </summary>
        public ICommand CloseCommand { get; private set; }

        private void CloseWindow(Window callerWindow)
        {
            callerWindow.Close();
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

        public bool GotoShowMenu
        {
            get { return _model.GotoShowMenu; }

            set
            {
                if (SetPropertyIndirect(() => _model.GotoShowMenu, (v) => _model.GotoShowMenu = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }

        public bool GotoCommandPrompt
        {
            get { return _model.GotoCommandPrompt; }

            set
            {
                if (SetPropertyIndirect(() => _model.GotoCommandPrompt, (v) => _model.GotoCommandPrompt = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }

        public bool GotoPowerShell
        {
            get { return _model.GotoPowerShell; }

            set
            {
                if (SetPropertyIndirect(() => _model.GotoPowerShell, (v) => _model.GotoPowerShell = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }

        public bool GotoExplorer
        {
            get { return _model.GotoExplorer; }

            set
            {
                if (SetPropertyIndirect(() => _model.GotoExplorer, (v) => _model.GotoExplorer = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }

        public bool HashShowMenu
        {
            get { return _model.HashShowMenu; }

            set
            {
                if (SetPropertyIndirect(() => _model.HashShowMenu, (v) => _model.HashShowMenu = v, value, false))
                {
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
                }
            }
        }

        public List<SystemFolderViewModel> SystemFolders
        {
            get
            {
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

                if (_systemFolders == null)
                {
                    _systemFolders = new List<SystemFolderViewModel>();

                    // Add temp path
                    _systemFolders.Add(new SystemFolderViewModel { Name = "Temp", Path = Path.GetTempPath() });

                    // List folders of interest
                    foreach (Environment.SpecialFolder folder in foldersOfInterest)
                    {
                        string path = Environment.GetFolderPath(folder);
                        if (!string.IsNullOrEmpty(path))
                            _systemFolders.Add(new SystemFolderViewModel { Name = folder.ToString(), Path = path });
                    }
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

        /// <summary>
        /// Gets a list of filenames, passed by the context menu.
        /// </summary>
        public List<FilepathViewModel> FilenamesForHash
        {
            get
            {
                if (_filenamesForHash == null)
                {
                    _filenamesForHash = new List<FilepathViewModel>();
                    foreach (string filename in _filenames)
                    {
                        if (File.Exists(filename))
                            _filenamesForHash.Add(new FilepathViewModel { Filepath = filename });
                    }
                    SelectedFilenameForHash = FilenamesForHash.FirstOrDefault();
                }
                return _filenamesForHash; 
            }
        }

        /// <summary>
        /// Gets or sets the user selected filename.
        /// </summary>
        public FilepathViewModel SelectedFilenameForHash
        {
            get { return _selectedFilenameForHash; }
            set 
            {
                if (SetProperty(ref _selectedFilenameForHash, value, false))
                {
                    HashResults.Clear();
                    var hashes = HashCalculator.CalculateHashes(SelectedFilenameForHash.Filepath);
                    foreach (HashCalculator.HashResult hash in hashes)
                        HashResults.Add(new HashResultViewModel { HashAlgorithm = hash.HashAlgorithm, HashValue = hash.HashValue });
                    UpdateHashHighlighting();
                }
            }
        }

        /// <summary>
        /// Gets a list of calculated hash values.
        /// </summary>
        public ObservableCollection<HashResultViewModel> HashResults { get; private set; }

        /// <summary>
        /// Gets or sets the user entered hash value to verify.
        /// </summary>
        public string HashCandidate
        {
            get { return _hashCandidate; }

            set 
            {
                if (SetProperty(ref _hashCandidate, value, false))
                {
                    UpdateHashHighlighting();
                }
            }
        }

        private void UpdateHashHighlighting()
        {
            string trimmedCandidate = HashCandidate?.Trim();

            // Update candidate highlighting
            if (string.IsNullOrEmpty(trimmedCandidate))
                HashVerified = null;
            else
                HashVerified = HashResults.Any(item => string.Equals(item.HashValue, trimmedCandidate, StringComparison.OrdinalIgnoreCase));

            // Update hash highlighting
            foreach (HashResultViewModel hashResult in HashResults)
            {
                if (string.IsNullOrEmpty(trimmedCandidate))
                    hashResult.Verified = null;
                else
                    hashResult.Verified = string.Equals(hashResult.HashValue, trimmedCandidate, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="HashCandidate"/> matches one of
        /// the <see cref="HashResults"/>.
        /// </summary>
        public bool? HashVerified
        {
            get { return _hashVerified; }
            set { SetProperty(ref _hashVerified, value, false); }
        }

        /// <summary>
        /// Gets the command which copies the hash to the clipboard.
        /// </summary>
        public ICommand CopyHashToClipboardCommand { get; private set; }

        private void CopyHashToClipboard(HashResultViewModel hashResult)
        {
            Clipboard.SetText(hashResult.HashValue);
        }

        /// <summary>
        /// Gets the command which inserts the hash from the clipboard into the <see cref="HashCandidate"/>.
        /// </summary>
        public ICommand PasteHashFromClipboardCommand { get; private set; }

        private void PasteHashFromClipboard(object obj)
        {
            string textFromClipboard = Clipboard.GetText();
            if (textFromClipboard != null)
            {
                string[] lines = textFromClipboard.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                textFromClipboard = lines.FirstOrDefault();
            }
            HashCandidate = textFromClipboard;
        }

        public string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
    }
}
