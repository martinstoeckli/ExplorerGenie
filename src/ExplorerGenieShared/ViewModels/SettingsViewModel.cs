// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Windows.Input;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// View model for the settings of ExplorerGenie.
    /// </summary>
    internal class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            OpenHomepageCommand = new RelayCommand(OpenHomepage);
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
