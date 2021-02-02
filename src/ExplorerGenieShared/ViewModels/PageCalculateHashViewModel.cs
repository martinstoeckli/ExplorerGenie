// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// ViewModel for the "Calculate hash" tab.
    /// </summary>
    internal class PageCalculateHashViewModel : ViewModelBaseWithLanguage
    {
        private readonly SettingsModel _model;
        private readonly ISettingsService _settingsService;
        private readonly List<string> _filenames;
        private List<FilenameOnlyViewModel> _filenamesForHash;
        private FilenameOnlyViewModel _selectedFilenameForHash;
        private string _hashCandidate;
        private bool? _hashVerified;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageCalculateHashViewModel"/> class.
        /// </summary>
        /// <param name="model">The shared model from <see cref="SettingsViewModel"/>.</param>
        /// <param name="language">The shared language service from <see cref="SettingsViewModel"/>.</param>
        /// <param name="settingsService">The shared settings service from <see cref="SettingsViewModel"/>.</param>
        /// <param name="filenames">User selected list of files, passed by the menu extension.</param>
        public PageCalculateHashViewModel(
            SettingsModel model,
            ILanguageService language,
            ISettingsService settingsService,
            List<string> filenames)
        {
            _model = model;
            Language = language;
            _settingsService = settingsService;
            _filenames = filenames;

            CopyHashToClipboardCommand = new RelayCommand<HashResultViewModel>(CopyHashToClipboard);
            PasteHashFromClipboardCommand = new RelayCommand(PasteHashFromClipboard);
            HashResults = new ObservableCollection<HashResultViewModel>();
        }

        /// <inheritdoc cref="SettingsModel.HashShowMenu"/>
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

        /// <summary>
        /// Gets a list of filenames, gotten from the context menu.
        /// </summary>
        public List<FilenameOnlyViewModel> FilenamesForHash
        {
            get
            {
                if (_filenamesForHash == null)
                {
                    _filenamesForHash = new List<FilenameOnlyViewModel>();
                    foreach (string filename in _filenames)
                    {
                        if (File.Exists(filename))
                            _filenamesForHash.Add(new FilenameOnlyViewModel { FullPath = filename });
                    }
                    SelectedFilenameForHash = FilenamesForHash.FirstOrDefault();
                }
                return _filenamesForHash;
            }
        }

        /// <summary>
        /// Gets or sets the user selected filename.
        /// </summary>
        public FilenameOnlyViewModel SelectedFilenameForHash
        {
            get { return _selectedFilenameForHash; }
            set
            {
                if (SetProperty(ref _selectedFilenameForHash, value, false))
                {
                    HashResults.Clear();
                    var hashes = HashCalculator.CalculateHashes(SelectedFilenameForHash.FullPath);
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
    }
}
