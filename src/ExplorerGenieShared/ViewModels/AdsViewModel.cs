// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// Collects information about the alternative datastreams of a single file/directory.
    /// </summary>
    public class AdsViewModel
    {
        public AdsViewModel()
        {
            Streams = new List<AdsStreamViewModel>();
        }

        public string Directory { get; set; }

        public string FileName { get; set; }

        public List<AdsStreamViewModel> Streams { get; set; }

        public int StreamCount
        {
            get { return Streams.Count; }
        }
    }

    public class AdsStreamViewModel
    {

    }
}
