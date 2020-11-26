// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// ViewModel for a single calculated hash value.
    /// </summary>
    public class HashResultViewModel : ViewModelBase
    {
        private string _hashAlgorithm;
        private string _hashValue;
        private bool? _verified;

        /// <summary>Gets or sets the name of the hash algorithm.</summary>
        public string HashAlgorithm
        {
            get { return _hashAlgorithm; }
            set { SetProperty(ref _hashAlgorithm, value, false); }
        }

        /// <summary>Gets or sets the calculated and encoded hash value.</summary>
        public string HashValue
        {
            get { return _hashValue; }
            set { SetProperty(ref _hashValue, value, false); }
        }

        /// <summary>Gets or sets a value indicating whether the hash matches the user entered hash.
        /// This can be used to show the status of the hash. The value null means that no check was
        /// done.</summary>
        public bool? Verified
        {
            get { return _verified; }
            set { SetProperty(ref _verified, value, false); }
        }
    }
}
