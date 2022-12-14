// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// Collects information about the alternative datastreams of a single file/directory.
    /// </summary>
    public class AdsViewModel
    {
        private List<FileStreamInfo> _streams;

        public AdsViewModel(string fullPath)
        {
            IsDirectory = Directory.Exists(fullPath);
            FullPath = fullPath;
            FileOrDirectoryName = Path.GetFileName(fullPath);
        }

        public bool IsDirectory { get; }

        public string FullPath { get; }

        public string FileOrDirectoryName { get; }

        public List<FileStreamInfo> Streams
        {
            get { return _streams ?? (_streams = CreateStreamList()); }
        }

        private List<FileStreamInfo> CreateStreamList()
        {
            return FileStreamSearcher.GetStreams(FullPath).ToList();
        }

        public int StreamCount
        {
            get { return Streams.Count; }
        }
    }
}
